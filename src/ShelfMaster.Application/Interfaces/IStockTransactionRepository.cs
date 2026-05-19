using ShelfMaster.Domain.Entities;

namespace ShelfMaster.Application.Interfaces;

public interface IStockTransactionRepository
{
    Task AddStockTransactionAsync(StockTransaction transaction);
    Task<StockTransaction?> GetStockTransactionByIdAsync(string id);
    Task<IEnumerable<StockTransaction>> GetAllStockTransactionsAsync();
    Task<IEnumerable<StockTransaction>> GetStockTransactionByUserIdAsync(string userId);
    Task<IEnumerable<StockTransaction>> GetStockTransactionByInventoryItemIdAsync(string inventoryItemId);
    Task DeleteStockTransactionAsync(string id);
}