using System.ComponentModel.DataAnnotations;

namespace ShelfMaster.Application.DTOs;
public record InventoryItemResponseDTO
(
    string Id,
    string Name,
    string SKU,
    int Quantity,
    decimal Price,
    int LowStockThreshold,
    DateTime CreatedAt,
    bool IsLowStock
);

public record CreateInventoryItemDTO
(
    [Required(ErrorMessage = "Item name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
    string Name,

    [Required(ErrorMessage = "SKU is required.")]
    [RegularExpression(@"^[A-Z0-9-]{5,15}$", ErrorMessage = "SKU must be 5-15 alphanumeric characters or hyphens.")]
    string SKU,

    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative integer.")]
    int Quantity,

    [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative decimal.")]
    decimal Price,

    [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold must be a non-negative integer.")]
    int LowStockThreshold
);

public record WithdrawInventoryItemDTO
(
    [Required(ErrorMessage = "Withdrawal amount is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Withdrawal amount must be a positive integer.")]
    int Amount
);

public record RestockInventoryItemDTO
(
    [Required(ErrorMessage = "Restock amount is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Restock amount must be a positive integer.")]
    int Amount
);

public record UpdateItemNameDTO
(
    [Required(ErrorMessage = "Item name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
    string Name
);

public record UpdateItemPriceDTO
(
    [Required(ErrorMessage = "Price is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative decimal.")]
    decimal Price
);

public record UpdateLowStockThresholdDTO
(
    [Required(ErrorMessage = "Low stock threshold is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold must be a non-negative integer.")]
    int LowStockThreshold
);

