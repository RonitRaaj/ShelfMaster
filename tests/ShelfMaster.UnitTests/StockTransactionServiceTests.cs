using Moq;
using FluentAssertions;
using Xunit;
using ShelfMaster.Application.Services;
using ShelfMaster.Application.Interfaces;
using ShelfMaster.Application.DTOs;
using ShelfMaster.Domain.Entities;
using ShelfMaster.Domain.Exceptions;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShelfMaster.UnitTests;

public class StockTransactionServiceTests
{
    private readonly Mock<IStockTransactionRepository> _transactionRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IInventoryRepository> _inventoryRepoMock;
    private readonly StockTransactionService _service;

    public StockTransactionServiceTests()
    {
        _transactionRepoMock = new Mock<IStockTransactionRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _inventoryRepoMock = new Mock<IInventoryRepository>();

        // System Under Test (SUT)
        _service = new StockTransactionService(
            _transactionRepoMock.Object,
            _userRepoMock.Object,
            _inventoryRepoMock.Object
        );
    }

    [Fact]
    public async Task AddStockTransactionAsync_ShouldCreateTransaction_WhenUserAndItemExist()
    {
        // Arrange
        var userId = "user-123";
        var itemId = "item-456";
        var dto = new StockTransactionRecordDTO(10, "Restock notes", itemId);

        var mockUser = new User("warehouse_guy", "guy@shelfmaster.com", "hash123");
        var mockItem = new InventoryItem("Apples","FR01", 50, 10, 10); // Assuming Name, Quantity, Threshold constructor parameters

        _userRepoMock.Setup(r => r.GetUserByIdAsync(userId))
            .ReturnsAsync(mockUser);
        _inventoryRepoMock.Setup(r => r.GetInventoryItemByIdAsync(itemId))
            .ReturnsAsync(mockItem);

        // Act
        var result = await _service.AddStockTransactionAsync(dto, userId);

        // Assert
        result.Should().NotBeNull();
        result.QuantityChange.Should().Be(10);
        result.Note.Should().Be("Restock notes");
        result.UserName.Should().Be("warehouse_guy");
        result.InventoryItemName.Should().Be("Apples");

        _transactionRepoMock.Verify(r => r.AddStockTransactionAsync(It.IsAny<StockTransaction>()), Times.Once);
    }

    [Fact]
    public async Task AddStockTransactionAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "invalid-user";
        var itemId = "item-456";
        var dto = new StockTransactionRecordDTO(10, "Notes", itemId);

        _userRepoMock.Setup(r => r.GetUserByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _service.AddStockTransactionAsync(dto, userId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with ID {userId} not found");

        _transactionRepoMock.Verify(r => r.AddStockTransactionAsync(It.IsAny<StockTransaction>()), Times.Never);
    }

    [Fact]
    public async Task AddStockTransactionAsync_ShouldThrowNotFoundException_WhenInventoryItemDoesNotExist()
    {
        // Arrange
        var userId = "user-123";
        var itemId = "invalid-item";
        var dto = new StockTransactionRecordDTO(5, "Notes", itemId);

        var mockUser = new User("warehouse_guy", "guy@shelfmaster.com", "hash123");

        _userRepoMock.Setup(r => r.GetUserByIdAsync(userId))
            .ReturnsAsync(mockUser);
        _inventoryRepoMock.Setup(r => r.GetInventoryItemByIdAsync(itemId))
            .ReturnsAsync((InventoryItem?)null);

        // Act
        Func<Task> act = async () => await _service.AddStockTransactionAsync(dto, userId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Inventory item with ID {itemId} not found");

        _transactionRepoMock.Verify(r => r.AddStockTransactionAsync(It.IsAny<StockTransaction>()), Times.Never);
    }
}