
using backend_purchase_order.Models.Enums;

namespace backend_purchase_order.Models.DTOs;

public class OrderActionDto
{
    public User User { get; private set; } = null!;
    public Order Order { get; private set; } = null!;

    public ActionType ActionType { get; set; }
}