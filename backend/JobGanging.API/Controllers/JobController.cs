using JobGanging.Core.Interfaces;
using JobGanging.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobGanging.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpPost("import-with-artwork")]
    public async Task<IActionResult> ImportJobWithArtwork([FromForm] IFormFile file, [FromForm] string? jobName, [FromForm] string? dieLineSpotColorName, [FromForm] int quantity = 1)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (fileExtension != ".pdf")
            return BadRequest("Only PDF files with artwork are supported for job import");

        // Validate inputs
        if (quantity <= 0)
            return BadRequest("Quantity must be greater than 0");

        if (!string.IsNullOrEmpty(jobName) && jobName.Length > 200)
            return BadRequest("Job name must be 200 characters or less");

        if (!string.IsNullOrEmpty(dieLineSpotColorName) && dieLineSpotColorName.Length > 100)
            return BadRequest("Spot color name must be 100 characters or less");

        var request = new JobImportRequest
        {
            JobName = jobName ?? Path.GetFileNameWithoutExtension(file.FileName),
            DieLineSpotColorName = dieLineSpotColorName ?? "CutContour",
            Quantity = quantity
        };

        using var stream = file.OpenReadStream();
        var result = await _jobService.ImportJobWithArtworkAsync(stream, file.FileName, request);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllJobs()
    {
        var jobs = await _jobService.GetAllJobsAsync();
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetJob(Guid id)
    {
        var job = await _jobService.GetJobAsync(id);
        if (job == null)
            return NotFound();

        return Ok(job);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJob(Guid id)
    {
        await _jobService.DeleteJobAsync(id);
        return NoContent();
    }
}
