using backend_purchase_order.Models.Enums;

namespace backend_purchase_order.Models.DTOs;

/// <summary>
/// DTO contendo um Id do pedido correspondente e um tipo indicando o tipo de ação
/// </summary>
public class RecordActionDto(int orderId, ActionType actionType)
{
    public int OrderId { get; } = orderId;
    public ActionType ActionType { get; } = actionType;
}