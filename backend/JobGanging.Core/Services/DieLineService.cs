using JobGanging.Core.Interfaces;
using JobGanging.Core.Models;
using System.Text;

namespace JobGanging.Core.Services;

public class DieLineService : IDieLineService
{
    private readonly List<DieLine> _dieLines = new();

    public async Task<DieLine> ImportDieLineAsync(Stream fileStream, string fileExtension, string fileName)
    {
        var dieLine = new DieLine
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            FileType = fileExtension.TrimStart('.').ToUpper(),
            CreatedAt = DateTime.UtcNow
        };

        // Read file data
        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();
        dieLine.RawData = Convert.ToBase64String(fileBytes);

        if (fileExtension == ".pdf")
        {
            dieLine = await ParsePdfDieLineAsync(fileBytes, dieLine);
        }
        else if (fileExtension == ".dxf")
        {
            dieLine = await ParseDxfDieLineAsync(fileBytes, dieLine);
        }

        _dieLines.Add(dieLine);
        return dieLine;
    }

    private async Task<DieLine> ParsePdfDieLineAsync(byte[] fileBytes, DieLine dieLine)
    {
        // TODO: Implement PDF parsing with spot color detection
        // For now, create a sample die line
        dieLine.Width = 100;
        dieLine.Height = 150;
        
        // Create a simple rectangular outline
        dieLine.Outline = new List<Point>
        {
            new Point { X = 0, Y = 0 },
            new Point { X = 100, Y = 0 },
            new Point { X = 100, Y = 150 },
            new Point { X = 0, Y = 150 }
        };

        // Sample spot colors
        dieLine.SpotColors = new List<SpotColor>
        {
            new SpotColor { Name = "CutContour", ColorValue = "#FF0000" }
        };

        return await Task.FromResult(dieLine);
    }

    private async Task<DieLine> ParseDxfDieLineAsync(byte[] fileBytes, DieLine dieLine)
    {
        // TODO: Implement DXF parsing with geometry extraction
        // For now, create a sample die line
        dieLine.Width = 120;
        dieLine.Height = 80;
        
        // Create a simple outline
        dieLine.Outline = new List<Point>
        {
            new Point { X = 0, Y = 0 },
            new Point { X = 120, Y = 0 },
            new Point { X = 120, Y = 80 },
            new Point { X = 0, Y = 80 }
        };

        return await Task.FromResult(dieLine);
    }

    public async Task<List<DieLine>> GetAllDieLinesAsync()
    {
        return await Task.FromResult(_dieLines.ToList());
    }

    public async Task<DieLine?> GetDieLineAsync(Guid id)
    {
        return await Task.FromResult(_dieLines.FirstOrDefault(d => d.Id == id));
    }

    public async Task DeleteDieLineAsync(Guid id)
    {
        var dieLine = _dieLines.FirstOrDefault(d => d.Id == id);
        if (dieLine != null)
        {
            _dieLines.Remove(dieLine);
        }
        await Task.CompletedTask;
    }
}
