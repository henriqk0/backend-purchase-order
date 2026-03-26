namespace backend_purchase_order.Models;

/// <summary>
/// Classe dos items.
/// O item contém um Id, um Nome e um valor
/// </summary>
public class Item
{
    public int Id { get; private set; }
    public double Value { get; private set; }
    public string Name { get; private set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<ItemOrder> ItemOrder { get; private set; } = [];

    public Item() { }

    // Os items não podem ter o preço nulo ou negativo
    public Item(double value, string name)
    {
        Update(value, name);
    }

    public void Update(double value, string name)
    {
        if (value <= 0)
            throw new ArgumentException("Invalid item value");
        Value = value;
        Name = name;
    }
}