using System.ComponentModel.DataAnnotations;

namespace ShelfMaster.Application.DTOs;

public record StockTransactionRecordDTO
(
    [Required(ErrorMessage = "Quantity change is required.")]
    int QuantityChange,
    string? Note,
    [Required(ErrorMessage = "Inventory Item ID is required.")]
    string InventoryItemId
);

public record StockTransactionResponseDTO
(
    string Id,
    int QuantityChange,
    DateTime Timestamp,
    string? Note,
    string UserId,
    string InventoryItemId,
    string? UserName,
    string? InventoryItemName 
);