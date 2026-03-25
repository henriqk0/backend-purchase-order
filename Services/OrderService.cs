using backend_purchase_order.Models;
using Microsoft.EntityFrameworkCore;
using backend_purchase_order.Models.DTOs;
using backend_purchase_order.Models.Enums;

namespace backend_purchase_order.Services;

public class OrderService(AppDbContext context, OrderActionHistoryService orderActionHistory)
{
    private readonly AppDbContext _context = context;
    private readonly OrderActionHistoryService _orderHistoryService = orderActionHistory;

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

        await _orderHistoryService.RecordActionInHistory(order, ActionType.Creation);

        return order;
    }
}