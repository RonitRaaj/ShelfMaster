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
}