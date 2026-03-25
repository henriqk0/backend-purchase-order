namespace backend_purchase_order.Models.DTOs;

/// <summary>
/// DTO contendo um Id do Item e a sua quantidade
/// </summary>
public class OrderItemDto
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}