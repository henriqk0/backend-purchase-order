namespace backend_purchase_order.Models.DTOs;

/// <summary>
/// DTO contendo uma lista de 
/// OrderItemDto, que contém Id do item e a sua quantidade
/// </summary>
public class OrderCreateDto
{
    public ICollection<OrderItemDto> Items { get; set; } = [];
}