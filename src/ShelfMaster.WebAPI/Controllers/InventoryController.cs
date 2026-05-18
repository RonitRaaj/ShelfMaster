using Microsoft.AspNetCore.Mvc;
using ShelfMaster.Application.DTOs;
using ShelfMaster.Application.Services;
namespace ShelfMaster.WebAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
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
}