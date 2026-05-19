namespace ShelfMaster.Domain.Entities;

public class StockTransaction
{
    public string Id { get; private set; }
    public int QuantityChanged { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? Notes { get; private set; }

    public string UserId { get; private set; } // Optional: Track which user made the transaction
    public User? User { get; private set; } // Navigation property for EF Core

    public string InventoryItemId { get; private set; } // Foreign key for InventoryItem
    public InventoryItem? InventoryItem { get; private set; } // Navigation property for EF

    private StockTransaction() 
    {
        Id = null!;
        UserId = null!;
        InventoryItemId = null!;
    } // For EF Core

    public StockTransaction(int quantityChanged, string? notes, string userId, string inventoryItemId)
    {
        if (quantityChanged == 0)
            throw new ArgumentException("Quantity changed cannot be zero.", nameof(quantityChanged));
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        if (string.IsNullOrWhiteSpace(inventoryItemId))
            throw new ArgumentException("Inventory Item ID cannot be empty.", nameof(inventoryItemId));

        Id = Guid.NewGuid().ToString();
        QuantityChanged = quantityChanged;
        Timestamp = DateTime.UtcNow;
        Notes = notes;
        UserId = userId;
        InventoryItemId = inventoryItemId;
    }
}