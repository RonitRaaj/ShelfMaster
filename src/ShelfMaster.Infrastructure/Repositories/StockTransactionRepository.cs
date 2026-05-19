using ShelfMaster.Infrastructure.Data;
using ShelfMaster.Application.Interfaces;
using ShelfMaster.Domain.Entities;
namespace ShelfMaster.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class StockTransactionRepository : IStockTransactionRepository
{
    private readonly AppDbContext _context;

    public StockTransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddStockTransactionAsync(StockTransaction transaction)
    {
        _context.StockTransactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<StockTransaction?> GetStockTransactionByIdAsync(string id)
    {
        return await _context.StockTransactions.Include(t => t.User).Include(t => t.InventoryItem).FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<StockTransaction>> GetAllStockTransactionsAsync()
    {
        return await _context.StockTransactions.Include(t => t.User).Include(t => t.InventoryItem).OrderByDescending(t => t.Timestamp).ToListAsync();
    }

    public async Task<IEnumerable<StockTransaction>> GetStockTransactionByUserIdAsync(string userId)
    {
        return await _context.StockTransactions.Where(t => t.UserId == userId).Include(t => t.User).Include(t => t.InventoryItem).OrderByDescending(t => t.Timestamp).ToListAsync();
    }

    public async Task<IEnumerable<StockTransaction>> GetStockTransactionByInventoryItemIdAsync(string inventoryItemId)
    {
        return await _context.StockTransactions.Where(t => t.InventoryItemId == inventoryItemId).Include(t => t.User).Include(t => t.InventoryItem).OrderByDescending(t => t.Timestamp).ToListAsync();
    }

    public async Task DeleteStockTransactionAsync(string id)
    {
        var transaction = await _context.StockTransactions.FindAsync(id);
        if (transaction != null)
        {
            _context.StockTransactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }
}