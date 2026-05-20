namespace ShelfMaster.Domain.Entities;

public class InventoryItem
{
    public string Id { get; init; }
    public string Name { get; private set; } = string.Empty;
    public string SKU { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public int LowStockThreshold { get; private set; }
    public DateTime CreatedAt { get; init; }

    private InventoryItem() 
    {
        Id = null!;
    } // For EF Core

    public InventoryItem(string name, string SKU, int quantity, decimal price, int lowStockThreshold)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        if(string.IsNullOrWhiteSpace(SKU))
            throw new ArgumentException("SKU cannot be empty.", nameof(SKU));
        if(quantity < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
        if(price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));
        if(lowStockThreshold < 0)
            throw new ArgumentException("Low stock threshold cannot be negative.", nameof(lowStockThreshold));
        if(lowStockThreshold > quantity)
            throw new ArgumentException("Low stock threshold cannot be greater than initial quantity.", nameof(lowStockThreshold));

        Id = Guid.NewGuid().ToString();
        Name = name;
        this.SKU = SKU;
        Quantity = quantity;
        Price = price;
        LowStockThreshold = lowStockThreshold;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string newName)
    {
        if(string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be empty.", nameof(newName));
        Name = newName;
    }

    public void RestockQuantity(int amount)
    {
        if(amount <= 0)
            throw new ArgumentException("Restock amount must be positive.", nameof(amount));
        Quantity += amount;
    }

    public void WithdrawQuantity(int amount)
    {
        if(amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive.", nameof(amount));
        if(amount > Quantity)
            throw new InvalidOperationException("Cannot withdraw more than available quantity.");
        Quantity -= amount;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if(newPrice < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(newPrice));
        Price = newPrice;
    }

    public void UpdateLowStockThreshold(int newLowStockThreshold)
    {
        if(newLowStockThreshold < 0)
            throw new ArgumentException("Low stock threshold cannot be negative.", nameof(newLowStockThreshold));
        LowStockThreshold = newLowStockThreshold;
    }

    public bool IsLowStock() => Quantity <= LowStockThreshold;
}