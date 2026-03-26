namespace backend_purchase_order.Models;

/// <summary>
/// Classe dos pedidos.
/// o Pedido possui um Usuário elaborador e pelo menos 1 item.
/// O atributo TotalValue representa o valor total dos itens e
/// foi colocado pois os itens podem mudar de preço em 
/// um momento futuro e isso nao se refletir no valor do pedido anterior
/// </summary>
public class Order
{
    public int Id { get; private set; }
    public ICollection<ItemOrder> ItemOrder { get; private set; } = [];
    public int OrderMakerId { get; private set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public User OrderMaker { get; private set; } = null!;
    public double TotalValue { get; private set; }

    public Order() { }

    public Order(IEnumerable<(Item item, int quantity)> items, int orderMakerId)
    {
        if (items == null || !items.Any() || orderMakerId == 0)
        {
            throw new ArgumentException("Order needs to have at least 1 item and a maker");
        }

        foreach (var item in items)
        {
            ItemOrder.Add(new ItemOrder(this, item.item, item.quantity));
        }
        CalculateTotalValue();

        OrderMakerId = orderMakerId;
    }

    private void CalculateTotalValue()
    {
        TotalValue = ItemOrder.Sum(io => io.Item.Value * io.Quantity);
    }
}