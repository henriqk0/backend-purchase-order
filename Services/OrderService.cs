using backend_purchase_order.Models;
using Microsoft.EntityFrameworkCore;
using backend_purchase_order.Models.DTOs;
using backend_purchase_order.Models.Enums;

namespace backend_purchase_order.Services;

public class OrderService(AppDbContext context, OrderActionHistoryService orderActionHistory)
{
    private readonly AppDbContext _context = context;
    private readonly OrderActionHistoryService _orderHistoryService = orderActionHistory;

    /// <summary>
    /// Para cada Id de Item passado no DTO, verifica se ele existe, e dá erro caso contrário,
    /// antes de vinculá-lo ao pedido
    /// </summary>
    /// <param name="orderDto">Paramentro contendo o Id do Usuário criador do pedido e uma lista de itens</param>
    /// <returns>O objeto Order Criado</returns>
    /// <exception cref="ArgumentException">Se o usuario do DTO nao exitir e nem ser colaborador ou se algum item nao existir</exception>
    public async Task<Order> CreateOrderAsync(OrderCreateDto orderDto)
    {
        var user = await _context.User.FindAsync(orderDto.OrderMakerId);

        if (user == null || user.Role != UserRole.Collaborator)
            throw new ArgumentException("Only existing collaborators can create orders.");

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

        var collaboratorActionDto = new RecordActionDto(order.OrderMakerId, order.Id, ActionType.Creation);

        await _orderHistoryService.RecordAgnosticAction(collaboratorActionDto);

        return order;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Order.ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Order.FindAsync(id);
    }

    public async Task UpdateOrderAsync(int id, Order order)
    {
        _context.Entry(order).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public bool OrderExists(int id)
    {
        return _context.Order.Any(e => e.Id == id);
    }

    public async Task DeleteOrderAsync(int id)
    {
        var order = await _context.Order.FindAsync(id);
        if (order != null)
        {
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}