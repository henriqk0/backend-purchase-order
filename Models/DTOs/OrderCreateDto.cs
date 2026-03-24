namespace backend_purchase_order.Models.DTOs;

public class OrderCreateDto
{
    public int OrderMakerId { get; set; }
    public ICollection<OrderItemDto> Items { get; set; } = [];
}