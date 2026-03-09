using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailAPI.Core.DTOs;
using RetailAPI.Core.Interfaces;

namespace RetailAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService) => _inventoryService = inventoryService;

    /// <summary>Gets current stock levels for all products. Admin only.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InventoryItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInventory()
    {
        var inventory = await _inventoryService.GetInventoryAsync();
        return Ok(inventory);
    }

    /// <summary>Adjusts stock quantity for a product. Admin only.</summary>
    /// <remarks>
    /// Use positive Adjustment to add stock (e.g., new shipment: +50).
    /// Use negative Adjustment to reduce stock (e.g., write-off: -5).
    /// </remarks>
    [HttpPatch("{productId:int}/stock")]
    [ProducesResponseType(typeof(InventoryItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdjustStock(int productId, [FromBody] StockAdjustmentRequest request)
    {
        var result = await _inventoryService.AdjustStockAsync(productId, request);
        return Ok(result);
    }
}
