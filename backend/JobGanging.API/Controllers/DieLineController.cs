using JobGanging.Core.Interfaces;
using JobGanging.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobGanging.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DieLineController : ControllerBase
{
    private readonly IDieLineService _dieLineService;

    public DieLineController(IDieLineService dieLineService)
    {
        _dieLineService = dieLineService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadDieLine([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (fileExtension != ".pdf" && fileExtension != ".dxf")
            return BadRequest("Only PDF and DXF files are supported");

        using var stream = file.OpenReadStream();
        var dieLine = await _dieLineService.ImportDieLineAsync(stream, fileExtension, file.FileName);

        return Ok(dieLine);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDieLines()
    {
        var dieLines = await _dieLineService.GetAllDieLinesAsync();
        return Ok(dieLines);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDieLine(Guid id)
    {
        var dieLine = await _dieLineService.GetDieLineAsync(id);
        if (dieLine == null)
            return NotFound();

        return Ok(dieLine);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDieLine(Guid id)
    {
        await _dieLineService.DeleteDieLineAsync(id);
        return NoContent();
    }
}
