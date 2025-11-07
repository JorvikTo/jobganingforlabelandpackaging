using JobGanging.Core.Interfaces;
using JobGanging.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobGanging.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SheetController : ControllerBase
{
    private readonly ISheetService _sheetService;

    public SheetController(ISheetService sheetService)
    {
        _sheetService = sheetService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSheet([FromBody] SheetRequest request)
    {
        var sheet = await _sheetService.CreateSheetAsync(request);
        return Ok(sheet);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSheets()
    {
        var sheets = await _sheetService.GetAllSheetsAsync();
        return Ok(sheets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSheet(Guid id)
    {
        var sheet = await _sheetService.GetSheetAsync(id);
        if (sheet == null)
            return NotFound();

        return Ok(sheet);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSheet(Guid id, [FromBody] SheetRequest request)
    {
        var sheet = await _sheetService.UpdateSheetAsync(id, request);
        return Ok(sheet);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSheet(Guid id)
    {
        await _sheetService.DeleteSheetAsync(id);
        return NoContent();
    }
}
