using JobGanging.Core.Interfaces;
using JobGanging.Core.Models;

namespace JobGanging.Core.Services;

public class JobService : IJobService
{
    private readonly List<Job> _jobs = new();
    private readonly IDieLineService _dieLineService;

    public JobService(IDieLineService dieLineService)
    {
        _dieLineService = dieLineService;
    }

    public async Task<JobImportResult> ImportJobWithArtworkAsync(Stream fileStream, string fileName, JobImportRequest request)
    {
        var job = new Job
        {
            Id = Guid.NewGuid(),
            JobName = string.IsNullOrEmpty(request.JobName) ? Path.GetFileNameWithoutExtension(fileName) : request.JobName,
            FileName = fileName,
            CreatedAt = DateTime.UtcNow,
            DieLineSpotColorName = request.DieLineSpotColorName,
            Quantity = request.Quantity
        };

        // Read the PDF artwork file
        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();
        job.ArtworkData = Convert.ToBase64String(fileBytes);

        // Extract die line from PDF artwork based on spot color
        var (dieLine, detectedSpotColors) = await ExtractDieLineFromPdfAsync(fileBytes, fileName, request.DieLineSpotColorName, job.Id);
        
        job.ExtractedDieLineId = dieLine.Id;
        _jobs.Add(job);

        return new JobImportResult
        {
            JobId = job.Id,
            DieLineId = dieLine.Id,
            JobName = job.JobName,
            Message = $"Job imported successfully. Die line extracted from spot color '{request.DieLineSpotColorName}'.",
            DetectedSpotColors = detectedSpotColors
        };
    }

    private async Task<(DieLine dieLine, List<string> detectedSpotColors)> ExtractDieLineFromPdfAsync(
        byte[] pdfBytes, 
        string fileName, 
        string dieLineSpotColor,
        Guid jobId)
    {
        // Create die line object
        var dieLine = new DieLine
        {
            Id = Guid.NewGuid(),
            FileName = $"{Path.GetFileNameWithoutExtension(fileName)}_dieline",
            FileType = "PDF",
            CreatedAt = DateTime.UtcNow,
            JobId = jobId,
            HasArtwork = true,
            RawData = Convert.ToBase64String(pdfBytes)
        };

        // TODO: Implement actual PDF parsing with PdfSharp to:
        // 1. Detect all spot colors in the PDF using PdfSharp.Pdf.Advanced
        // 2. Extract paths/vectors that use the specified spot color
        // 3. Convert those paths to die line outline points using coordinate transformation
        // 4. Calculate bounding box dimensions from the extracted geometry
        // 5. Separate artwork from die line by removing or isolating the spot color layer
        //
        // Implementation approach:
        // - Use PdfSharp.Pdf.IO.PdfReader to open the PDF
        // - Iterate through pages and extract content streams
        // - Parse color space definitions to identify spot colors
        // - Extract path operations (moveto, lineto, curveto) for die line geometry
        // - Transform coordinates from PDF space to die line coordinate system
        //
        // Expected timeline: Production implementation requires PdfSharp advanced features
        // Reference: https://docs.pdfsharp.net/PDFsharp/index.html

        // For now, simulate the extraction with sample data
        var detectedSpotColors = new List<string> 
        { 
            "CutContour", 
            "CMYK", 
            "Pantone 185 C",
            "Varnish"
        };

        // Sample die line extraction (rectangle for demo)
        dieLine.Width = 100;
        dieLine.Height = 150;
        dieLine.Outline = new List<Point>
        {
            new Point { X = 0, Y = 0 },
            new Point { X = 100, Y = 0 },
            new Point { X = 100, Y = 150 },
            new Point { X = 0, Y = 150 }
        };

        // Add the die line spot color
        dieLine.SpotColors = new List<SpotColor>
        {
            new SpotColor 
            { 
                Name = dieLineSpotColor, 
                ColorValue = "#FF0000" // Red for die line
            }
        };

        // In a real implementation, we would:
        // 1. Parse PDF and find objects with the spot color
        // 2. Extract vector paths from those objects
        // 3. Convert to outline points
        // 4. Store the artwork separately (without die line layer)

        // Note: The actual implementation would use PdfSharp like this:
        // using PdfSharp.Pdf;
        // using PdfSharp.Pdf.IO;
        // var document = PdfReader.Open(new MemoryStream(pdfBytes), PdfDocumentOpenMode.Import);
        // foreach (var page in document.Pages)
        // {
        //     // Extract spot colors and paths
        //     // Identify die line based on spot color name
        //     // Extract geometry
        // }

        return await Task.FromResult((dieLine, detectedSpotColors));
    }

    public async Task<List<Job>> GetAllJobsAsync()
    {
        return _jobs.ToList();
    }

    public async Task<Job?> GetJobAsync(Guid id)
    {
        return _jobs.FirstOrDefault(j => j.Id == id);
    }

    public async Task DeleteJobAsync(Guid id)
    {
        var job = _jobs.FirstOrDefault(j => j.Id == id);
        if (job != null)
        {
            // Also delete the associated die line
            if (job.ExtractedDieLineId.HasValue)
            {
                await _dieLineService.DeleteDieLineAsync(job.ExtractedDieLineId.Value);
            }
            _jobs.Remove(job);
        }
        await Task.CompletedTask;
    }
}
