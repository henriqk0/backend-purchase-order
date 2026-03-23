namespace backend_purchase_order.Models;

/// <summary>
/// Classe dos pedidos.
/// o Pedido possui um Usuário elaborador e pelo menos 1 item.
/// O atributo TotalValue representa o valor total dos itens e
/// foi colocado pois os itens podem mudar de preço em 
/// um pedido futuro, e pode ser interessante saber essa diferença  refletida no 
/// valor do pedido
/// </summary>
public class Order
{
    public int Id { get; private set; }
    public ICollection<ItemOrder> ItemOrder { get; private set; } = [];
    public int OrderMakerId { get; private set; }
    public User OrderMaker { get; private set; } = null!;
    public double TotalValue { get; private set; }

    public Order() { }

    public Order(IEnumerable<(Item item, int quantity)> items)
    {
        if (items == null || !items.Any())
        {
            throw new ArgumentException("Order needs to have at least 1 item");
        }

        foreach (var item in items)
        {
            ItemOrder.Add(new ItemOrder(this, item.item, item.quantity));
        }
    }
}