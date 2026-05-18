using Moq;
using Xunit;
using ShelfMaster.Application.DTOs;
using ShelfMaster.Application.Interfaces;
using ShelfMaster.Application.Services;
using ShelfMaster.Domain.Entities;
using ShelfMaster.Domain.Exceptions;

namespace ShelfMaster.UnitTests.Application;

public class InventoryServiceTests
{
    private readonly Mock<IInventoryRepository> _repositoryMock;
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        _repositoryMock = new Mock<IInventoryRepository>();
        _service = new InventoryService(_repositoryMock.Object);
    }

    [Fact]
    public async Task AddInventoryItemAsync_ShouldThrowValidationException_WhenSkuAlreadyExists()
    {
        var duplicateSku = "SKU-DUPE123";
        var dto = new CreateInventoryItemDTO("New Item", duplicateSku, 10, 15.00m, 2);
        var existingItem = new InventoryItem("Existing Item", duplicateSku, 5, 20.00m, 1);

        _repositoryMock.Setup(repo => repo.GetBySKUAsync(duplicateSku))
                       .ReturnsAsync(existingItem);

        await Assert.ThrowsAsync<ValidationException>(() => _service.AddInventoryItemAsync(dto));
        
        _repositoryMock.Verify(repo => repo.AddInventoryItemAsync(It.IsAny<InventoryItem>()), Times.Never);
    }

    [Fact]
    public async Task WithdrawInventoryItemAsync_ShouldUpdateRepository_WhenWithdrawalIsValid()
    {
        var itemId = "item-guid-123";
        var existingItem = new InventoryItem("Gadget", "SKU-GADG1", 20, 10.00m, 5);
        
        var dto = new WithdrawInventoryItemDTO(5);

        _repositoryMock.Setup(repo => repo.GetInventoryItemByIdAsync(itemId))
                       .ReturnsAsync(existingItem);


        var remainingQuantity = await _service.WithdrawInventoryItemAsync(itemId, dto);

        Assert.Equal(15, remainingQuantity);
        _repositoryMock.Verify(repo => repo.UpdateInventoryItemAsync(existingItem), Times.Once);
    }
}