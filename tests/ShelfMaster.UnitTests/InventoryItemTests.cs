using Xunit;
using ShelfMaster.Domain.Entities;

namespace ShelfMaster.UnitTests.Domain;

public class InventoryItemTests
{
    [Fact]
    public void RestockQuantity_ShouldIncreaseQuantity_WhenAmountIsPositive()
    {
        // Arrange
        var item = new InventoryItem("Test Item", "SKU-12345", 10, 99.99m, 5);

        // Act
        item.RestockQuantity(5);

        // Assert
        Assert.Equal(15, item.Quantity);
    }

    [Fact]
    public void WithdrawQuantity_ShouldThrowException_WhenAmountExceedsAvailableStock()
    {
        // Arrange
        var item = new InventoryItem("Test Item", "SKU-12345", 10, 99.99m, 5);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => item.WithdrawQuantity(15));
        Assert.Contains("Cannot withdraw more than available", exception.Message);
    }
}