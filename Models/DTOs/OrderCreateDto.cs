namespace backend_purchase_order.Models.DTOs;

/// <summary>
/// DTO contendo um Id de Usuário fazedor do Pedido e uma lista de 
/// OrderItemDto, que contém Id do item e a sua quantidade
/// </summary>
public class OrderCreateDto
{
    public int OrderMakerId { get; set; }
    public ICollection<OrderItemDto> Items { get; set; } = [];
}