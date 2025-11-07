using JobGanging.Core.Interfaces;
using JobGanging.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobGanging.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;

    public ExportController(IExportService exportService)
    {
        _exportService = exportService;
    }

    [HttpPost("pdf")]
    public async Task<IActionResult> ExportToPdf([FromBody] ExportRequest request)
    {
        var pdfBytes = await _exportService.ExportToPdfAsync(request);
        return File(pdfBytes, "application/pdf", $"ganging-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
    }

    [HttpPost("report")]
    public async Task<IActionResult> GenerateReport([FromBody] ExportRequest request)
    {
        var csvBytes = await _exportService.GenerateReportAsync(request);
        return File(csvBytes, "text/csv", $"report-{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
    }
}
