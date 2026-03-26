namespace backend_purchase_order.Models.DTOs;

public class OrderUpdateDto
{
    public ICollection<OrderItemDto> Items { get; set; } = [];
}