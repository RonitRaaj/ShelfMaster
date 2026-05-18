using ShelfMaster.Infrastructure.Data;
using ShelfMaster.Application.Interfaces;
using ShelfMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace ShelfMaster.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext _context;

    public InventoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddInventoryItemAsync(InventoryItem item)
    {
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task<InventoryItem?> GetInventoryItemByIdAsync(string id)
    {
        return await _context.InventoryItems.FindAsync(id);
    }

    public async Task<IEnumerable<InventoryItem>> GetAllInventoryItemsAsync()
    {
        return await _context.InventoryItems.ToListAsync();
    }

    public async Task UpdateInventoryItemAsync(InventoryItem item)
    {
        _context.InventoryItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteInventoryItemAsync(string id)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item != null)
        {
            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<InventoryItem?> GetBySKUAsync(string SKU)
    {
        return await _context.InventoryItems.FirstOrDefaultAsync(i => i.SKU == SKU);
    }
}