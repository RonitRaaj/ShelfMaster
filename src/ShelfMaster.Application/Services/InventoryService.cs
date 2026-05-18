using ShelfMaster.Application.DTOs;
using ShelfMaster.Application.Interfaces;
using ShelfMaster.Domain.Entities;
using ShelfMaster.Domain.Exceptions;
namespace ShelfMaster.Application.Services;

public class InventoryService
{
    private readonly IInventoryRepository _repository;

    public InventoryService(IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<InventoryItemResponseDTO> AddInventoryItemAsync(CreateInventoryItemDTO dto)
    {
        var itemWithSameSKU = await _repository.GetBySKUAsync(dto.SKU);
        if (itemWithSameSKU != null) throw new ValidationException($"An inventory item with SKU {dto.SKU} already exists.");
        var item = new InventoryItem(dto.Name, dto.SKU, dto.Quantity, dto.Price, dto.LowStockThreshold);
        await _repository.AddInventoryItemAsync(item);
        return new InventoryItemResponseDTO(
            item.Id,
            item.Name,
            item.SKU,
            item.Quantity,
            item.Price,
            item.LowStockThreshold,
            item.CreatedAt,
            item.IsLowStock()
        );
    }

    public async Task<InventoryItemResponseDTO?> GetInventoryItemByIdAsync(string id)
    {
        var item = await _repository.GetInventoryItemByIdAsync(id);
        if (item == null) throw new NotFoundException($"Inventory item with ID {id} not found");

        return new InventoryItemResponseDTO(
            item.Id,
            item.Name,
            item.SKU,
            item.Quantity,
            item.Price,
            item.LowStockThreshold,
            item.CreatedAt,
            item.IsLowStock()
        );
    }

    public async Task<IEnumerable<InventoryItemResponseDTO>> GetAllInventoryItemsAsync()
    {
        var items = await _repository.GetAllInventoryItemsAsync();
        return items.Select(item => new InventoryItemResponseDTO(
            item.Id,
            item.Name,
            item.SKU,
            item.Quantity,
            item.Price,
            item.LowStockThreshold,
            item.CreatedAt,
            item.IsLowStock()
        ));
    }
    
    public async Task<IEnumerable<InventoryItemResponseDTO>> GetAllAvailableInventoryItemsAsync()
    {
        var items = await _repository.GetAllInventoryItemsAsync();
        return items.Where(item => item.Quantity > 0).Select(item => new InventoryItemResponseDTO(
            item.Id,
            item.Name,
            item.SKU,
            item.Quantity,
            item.Price,
            item.LowStockThreshold,
            item.CreatedAt,
            item.IsLowStock()
        )).ToList();
    }

    public async Task<int> WithdrawInventoryItemAsync(string id, WithdrawInventoryItemDTO dto)
    {
        var item = await _repository.GetInventoryItemByIdAsync(id);
        if (item == null) throw new NotFoundException($"Inventory item with ID {id} not found");
        item.WithdrawQuantity(dto.Amount);
        await _repository.UpdateInventoryItemAsync(item);
        return item.Quantity;
    }

    public async Task<int> RestockInventoryItemAsync(string id, RestockInventoryItemDTO dto)
    {
        var item = await _repository.GetInventoryItemByIdAsync(id);
        if (item == null) throw new NotFoundException($"Inventory item with ID {id} not found");
        item.RestockQuantity(dto.Amount);
        await _repository.UpdateInventoryItemAsync(item);
        return item.Quantity;
    }

    public async Task<string> UpdateItemNameAsync(string id, UpdateItemNameDTO dto)
    {
        var item = await _repository.GetInventoryItemByIdAsync(id);
        if (item == null) throw new NotFoundException($"Inventory item with ID {id} not found");
        item.UpdateName(dto.Name);
        await _repository.UpdateInventoryItemAsync(item);
        return item.Name;
    }

    public async Task<decimal> UpdateItemPriceAsync(string id, UpdateItemPriceDTO dto)
    {
        var item = await _repository.GetInventoryItemByIdAsync(id);
        if (item == null) throw new NotFoundException($"Inventory item with ID {id} not found");
        item.UpdatePrice(dto.Price);
        await _repository.UpdateInventoryItemAsync(item);
        return item.Price;
    }

    public async Task<int> UpdateItemLowStockThresholdAsync(string id, UpdateLowStockThresholdDTO dto)
    {
        var item = await _repository.GetInventoryItemByIdAsync(id);
        if (item == null) throw new NotFoundException($"Inventory item with ID {id} not found");
        item.UpdateLowStockThreshold(dto.LowStockThreshold);
        await _repository.UpdateInventoryItemAsync(item);
        return item.LowStockThreshold;
    }
    
    public async Task DeleteInventoryItemAsync(string id)
    {
        var item = await _repository.GetInventoryItemByIdAsync(id);
        if (item == null) throw new NotFoundException($"Inventory item with ID {id} not found");
        await _repository.DeleteInventoryItemAsync(id);
    }
}