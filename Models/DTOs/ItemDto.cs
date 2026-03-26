namespace backend_purchase_order.Models.DTOs;

/// <summary>
/// DTO contendo o Valor do Item e seu Nome
/// </summary>
public class ItemDto
{
    public double Value { get; set; }
    public string Name { get; set; } = null!;
}