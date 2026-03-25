using backend_purchase_order.Models.Enums;

namespace backend_purchase_order.Models.DTOs;

/// <summary>
/// DTO contendo um Id de Usuário fazedor da ação, um Id do
/// pedido correspondente (ambos devem ser checados se são válidos
/// sempre que necessário) e um tipo indicando o tipo de ação
/// </summary>
public class RecordActionDto(int actionMakerId, int orderId, ActionType actionType)
{
    public int ActionMakerId { get; } = actionMakerId;
    public int OrderId { get; } = orderId;
    public ActionType ActionType { get; } = actionType;
}