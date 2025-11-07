using JobGanging.Core.Interfaces;
using JobGanging.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobGanging.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NestingController : ControllerBase
{
    private readonly INestingService _nestingService;

    public NestingController(INestingService nestingService)
    {
        _nestingService = nestingService;
    }

    [HttpPost("optimize")]
    public async Task<IActionResult> OptimizeLayout([FromBody] NestingRequest request)
    {
        var result = await _nestingService.OptimizeLayoutAsync(request);
        return Ok(result);
    }

    [HttpPost("manual-adjust")]
    public async Task<IActionResult> ManualAdjust([FromBody] ManualAdjustmentRequest request)
    {
        var result = await _nestingService.ManualAdjustAsync(request);
        return Ok(result);
    }

    [HttpPost("calculate-waste")]
    public async Task<IActionResult> CalculateWaste([FromBody] WasteCalculationRequest request)
    {
        var waste = await _nestingService.CalculateWasteAsync(request);
        return Ok(waste);
    }
}
