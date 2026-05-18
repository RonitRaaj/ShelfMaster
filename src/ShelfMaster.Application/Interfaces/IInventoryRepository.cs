using ShelfMaster.Domain.Entities;
namespace ShelfMaster.Application.Interfaces;

public interface IInventoryRepository
{
    Task AddInventoryItemAsync(InventoryItem item);
    Task<InventoryItem?> GetInventoryItemByIdAsync(string id);
    Task<IEnumerable<InventoryItem>> GetAllInventoryItemsAsync();
    Task UpdateInventoryItemAsync(InventoryItem item);
    Task DeleteInventoryItemAsync(string id);
}