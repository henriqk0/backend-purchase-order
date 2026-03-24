using backend_purchase_order.Models;
using Microsoft.EntityFrameworkCore;
using backend_purchase_order.Models.DTOs;
using backend_purchase_order.Models.Enums;

namespace backend_purchase_order.Services;

public class OrderService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<Order> CreateOrderAsync(OrderCreateDto orderDto)
    {
        var itemsForOrder = new List<(Item item, int quantity)>();

        foreach (var orderItemDto in orderDto.Items)
        {
            var item = await _context.Item.FindAsync(orderItemDto.ItemId)
                ?? throw new ArgumentException($"Item with ID {orderItemDto.ItemId} does not exist.");

            itemsForOrder.Add((item, orderItemDto.Quantity));
        }

        var order = new Order(itemsForOrder, orderDto.OrderMakerId);

        _context.Order.Add(order);

        await _context.SaveChangesAsync();

        await RecordActionInHistory(order, ActionType.Creation);

        return order;
    }

    public async Task<OrderActionHistory> RecordActionInHistory(Order order, ActionType actionType)
    {
        DateTime nowAction = DateTime.Now;

        DateOnly currentDate = DateOnly.FromDateTime(nowAction);
        TimeOnly currentTime = TimeOnly.FromDateTime(nowAction);

        var orderHistory = new OrderActionHistory(order, order.OrderMaker, currentDate, currentTime, actionType);
        _context.OrderActionHistory.Add(orderHistory);

        await _context.SaveChangesAsync();

        return orderHistory;
    }

    public async Task<OrderActionHistory> RecordApproverAction(OrderActionDto orderActionDto)
    {
        var lastActionAtOrder = await _context.OrderActionHistory
            .Where(orderActHist => orderActHist.OrderId == orderActionDto.Order.Id)
            .OrderByDescending(orderActHist => orderActHist.Id)
            .FirstOrDefaultAsync();

        // Se o pedido foi concluido ou se aguarda revisão ou se não foi criado, o aprovador não pode tomar uma ação
        if (lastActionAtOrder == null || lastActionAtOrder.ActionType == ActionType.Concluison
            || lastActionAtOrder.ActionType == ActionType.RequestForReview)
            throw new ArgumentException($"Invalid order to take an action");


        // Se o inteiro de ação recebido no DTO não corresponder a uma ação enumerada, também levanta um erro
        if (!Enum.IsDefined(typeof(ActionType), orderActionDto.ActionType))
            throw new ArgumentException("Invalid action");

        var userRoleInLastOrderAction = lastActionAtOrder.ResponsibleUser.Role;
        var userRoleFromDTO = orderActionDto.User.Role;
        var price = lastActionAtOrder.Order.TotalValue;

        // Nenhum usuário de setor algum pode tomar uma ação se o setor anterior não o tomou.
        // Usuários do mesmo setor também não podem alterar uma definição já existente pelo mesmo setor
        if (userRoleFromDTO == userRoleInLastOrderAction || userRoleFromDTO != userRoleInLastOrderAction + 1)
            throw new ArgumentException("Action cannot be taken before the lower hierarchy");

        // Se o pedido for de aprovação, então podemos concluí-lo dependendo das aprovações anteriores:
        if (orderActionDto.ActionType == ActionType.Approbation)
            // Se o preco for inferior a 100, também conclui o pedido
            if (price <= 100)
                await RecordActionInHistory(orderActionDto.Order, ActionType.Concluison);
            // Caso caso esteja entre 100.00000...1 e 1000, e o setor de Suprimentos já aprovou
            else if (price <= 1000 && userRoleInLastOrderAction == UserRole.SupplyDepartment)
                await RecordActionInHistory(orderActionDto.Order, ActionType.Concluison);
            // Caso contrário e o diretor é o último a aprovar, então também podemos concluir o pedido. 
            else if (userRoleInLastOrderAction == UserRole.Manager)
                await RecordActionInHistory(orderActionDto.Order, ActionType.Concluison);

        return await RecordActionInHistory(orderActionDto.Order, orderActionDto.ActionType);
    }
}