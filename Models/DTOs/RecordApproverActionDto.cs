using backend_purchase_order.Models.Enums;

namespace backend_purchase_order.Models.DTOs;

/// <summary>
/// DTO contendo um Id do pedido correspondente. Ambos devem ser checados se são válidos
/// sempre que necessário. Também contém um atributo enum representando o tipo da ação 
/// e outro representando a role do usuário
/// </summary>
public class RecordApproverActionDto(int orderId, ActionType actionType, UserRole approverRole)
{
    public int OrderId { get; } = orderId;
    public ActionType ActionType { get; } = actionType;
    public UserRole ApproverRole { get; } = approverRole;
}