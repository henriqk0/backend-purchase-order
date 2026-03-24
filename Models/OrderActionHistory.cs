
using backend_purchase_order.Models.Enums;

namespace backend_purchase_order.Models;

/// <summary>
/// Classe de relacionamento entre Usuários e Pedidos.
/// Cada pedido pode estar relacionado a vários usuários
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

    // Para garantir que há instancias de Pedido e de Usuário para a construção do objeto
    // Se order ou user recebidos for nulos, ou se o inteiro da acao nao existir no enum,
    // ou se a data + hora da ação for no futuro, levanta um erro
    public OrderActionHistory(Order order, User user, DateOnly dateAction, TimeOnly timeAction, ActionType actionType)
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        ResponsibleUser = user ?? throw new ArgumentNullException(nameof(user));
        var dateTime = dateAction.ToDateTime(timeAction);

        if (dateTime < DateTime.Now)
            throw new ArgumentException("Invalid schedule");

        DateAction = dateAction;
        TimeAction = timeAction;

        if (!Enum.IsDefined(typeof(ActionType), actionType))
            throw new ArgumentException("Invalid action");

        ActionType = actionType;
    }
}