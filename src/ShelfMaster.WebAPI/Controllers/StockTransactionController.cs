using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShelfMaster.Application.DTOs;
using ShelfMaster.Application.Services;

namespace ShelfMaster.WebAPI.Controllers;


public class StockTransactionController : BaseApiController
{
    private readonly StockTransactionService _service;

    public StockTransactionController(StockTransactionService service)
    {
        _service = service;
    }

    [Authorize (Roles = "Admin,Staff")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateStockTransaction([FromBody] StockTransactionRecordDTO dto)
    {
        var result = await _service.AddStockTransactionAsync(dto, CurrentUserId);
        return Ok(result);
    }
    
    [Authorize (Roles = "Admin,Staff")]
    [HttpGet("{id}", Name = "GetStockTransactionById")]
    public async Task<IActionResult> GetStockTransactionById(string id)
    {
        var transaction = await _service.GetStockTransactionByIdAsync(id);
        if (transaction == null) return NotFound();
        return Ok(transaction);
    }

    [Authorize (Roles = "Admin,Staff")]
    [HttpGet("list")]
    public async Task<IActionResult> GetAllStockTransactions()
    {
        var transactions = await _service.GetAllStockTransactionsAsync();
        return Ok(transactions);
    }

    [Authorize (Roles = "Admin,Staff")]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetStockTransactionsByUserId(string userId)
    {
        var transactions = await _service.GetStockTransactionsByUserIdAsync(userId);
        return Ok(transactions);
    }

    [Authorize (Roles = "Admin,Staff")]
    [HttpGet("item/{itemId}")]
    public async Task<IActionResult> GetStockTransactionsByInventoryItemId(string itemId)
    {
        var transactions = await _service.GetStockTransactionsByInventoryItemIdAsync(itemId);
        return Ok(transactions);
    }

    [Authorize (Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStockTransaction(string id)
    {
        await _service.DeleteStockTransactionAsync(id);
        return Ok(new { message = "Stock transaction deleted successfully" });
    }

}