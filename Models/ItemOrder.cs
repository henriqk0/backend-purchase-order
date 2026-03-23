namespace backend_purchase_order.Models;

/// <summary>
/// Classe de relacionamento entre Pedidos e Itens.
/// Cada pedido pode estar relacionado a vários itens
/// </summary>
public class ItemOrder
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public int ItemId { get; private set; }
    public Item Item { get; private set; } = null!;

    public int Quantity { get; private set; }

    public ItemOrder() { }

    // Só permite criar 1 registro de ItemOrder se se passar 1 objeto Order e 1 objeto Item, sendo este último
    // com pelo menos 1 quantidade de item.
    public ItemOrder(Order order, Item item, int quantity)
    {
        if (quantity < 1)
            throw new ArgumentException("Invalid quantity");

        Quantity = quantity;
        Order = order ?? throw new ArgumentNullException(nameof(order));
        Item = item ?? throw new ArgumentNullException(nameof(item));
    }
}
