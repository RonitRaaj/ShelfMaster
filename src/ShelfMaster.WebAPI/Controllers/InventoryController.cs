using Microsoft.AspNetCore.Mvc;
using ShelfMaster.Application.DTOs;
using ShelfMaster.Application.Services;
namespace ShelfMaster.WebAPI.Controllers;


public class InventoryController : BaseApiController
{
    private readonly InventoryService _service;

    public InventoryController(InventoryService service)
    {
        _service = service;
    }

    [HttpPost("items/create")]
    public async Task<IActionResult> AddInventoryItem([FromBody] CreateInventoryItemDTO dto)
    {
        var result = await _service.AddInventoryItemAsync(dto);
        return CreatedAtAction("GetInventoryItemById", new { id = result.Id }, result);
    }

    [HttpGet("items/search/{id}" , Name = "GetInventoryItemById")]
    public async Task<IActionResult> GetInventoryItemById(string id)
    {
        var item = await _service.GetInventoryItemByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpGet("items/list")]
    public async Task<IActionResult> GetAllInventoryItems()
    {
        var items = await _service.GetAllInventoryItemsAsync();
        return Ok(items);
    }
    
    [HttpGet("items/available")]
    public async Task<IActionResult> GetAllAvailableInventoryItems()
    {
        var items = await _service.GetAllAvailableInventoryItemsAsync();
        return Ok(items);
    }

    [HttpPatch("items/{id}/restock")]
    public async Task<IActionResult> RestockInventoryItem(string id, [FromBody] RestockInventoryItemDTO dto)
    {
        var newQuantity = await _service.RestockInventoryItemAsync(id, dto);
        return Ok(new{message = "Inventory item restocked successfully", quantity = newQuantity});
    }

    [HttpPatch("items/{id}/withdraw")]
    public async Task<IActionResult> WithdrawInventoryItem(string id, [FromBody] WithdrawInventoryItemDTO dto)
    {
        var newQuantity = await _service.WithdrawInventoryItemAsync(id, dto);
        return Ok(new{message = "Inventory item withdrawn successfully", quantity = newQuantity});
    }

    [HttpDelete("items/{id}/delete")]
    public async Task<IActionResult> DeleteInventoryItem(string id)
    {
        await _service.DeleteInventoryItemAsync(id);
        return Ok(new{message = "Inventory item deleted successfully"});
    }

    [HttpPatch("items/{id}/update-name")]
    public async Task<IActionResult> UpdateItemName(string id, [FromBody] UpdateItemNameDTO dto)
    {
        var newName = await _service.UpdateItemNameAsync(id, dto);
        return Ok(new{message = "Inventory item name updated successfully", name = newName});
    }

    [HttpPatch("items/{id}/update-price")]
    public async Task<IActionResult> UpdateItemPrice(string id, [FromBody] UpdateItemPriceDTO dto)
    {
        var newPrice = await _service.UpdateItemPriceAsync(id, dto);
        return Ok(new{message = "Inventory item price updated successfully", price = newPrice});
    }

    [HttpPatch("items/{id}/update-low-stock-threshold")]
    public async Task<IActionResult> UpdateItemLowStockThreshold(string id, [FromBody] UpdateLowStockThresholdDTO dto)
    {
        var newThreshold = await _service.UpdateItemLowStockThresholdAsync(id, dto);
        return Ok(new{message = "Inventory item low stock threshold updated successfully", threshold = newThreshold});
    }
}