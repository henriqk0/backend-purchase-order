
using backend_purchase_order.Models.Enums;

namespace backend_purchase_order.Models;

/// <summary>
/// Classe de relacionamento entre diversos Usuários e diversos Pedidos 
/// com respeito ao tipo de ação e o momento em que foi feito
/// </summary>
public class OrderActionHistory
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public Order Order { get; private set; } = null!;
    public int UserId { get; private set; }
    public User ResponsibleUser { get; private set; } = null!;
    public ActionType ActionType { get; private set; }
    public DateOnly DateAction { get; private set; }
    public TimeOnly TimeAction { get; private set; }

    public OrderActionHistory() { }

    // O construtor da entidade não verifica se o userId e o orderId existem atualmente, pois isto subentende-se estar sendo 
    // verificado pelo banco e optou-se por nao levantar um erro antes disso
    public OrderActionHistory(int orderId, int userId, DateOnly dateAction, TimeOnly timeAction, ActionType actionType)
    {
        var dateTime = dateAction.ToDateTime(timeAction);

        if (dateTime < DateTime.Now.AddMinutes(-1))
            throw new ArgumentException("Invalid schedule");

        DateAction = dateAction;
        TimeAction = timeAction;

        if (!Enum.IsDefined(typeof(ActionType), actionType))
            throw new ArgumentException("Invalid action");

        OrderId = orderId;
        UserId = userId;

        ActionType = actionType;
    }
}