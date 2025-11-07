using JobGanging.Core.Interfaces;
using JobGanging.Core.Models;
using System.Text;

namespace JobGanging.Core.Services;

public class ExportService : IExportService
{
    private readonly ISheetService _sheetService;
    private readonly IDieLineService _dieLineService;

    public ExportService(ISheetService sheetService, IDieLineService dieLineService)
    {
        _sheetService = sheetService;
        _dieLineService = dieLineService;
    }

    public async Task<byte[]> ExportToPdfAsync(ExportRequest request)
    {
        // TODO: Implement PDF export with PdfSharp
        // For now, return a placeholder
        var content = "PDF Export - Sheet Layout with Die Lines\n";
        content += $"Sheet ID: {request.SheetId}\n";
        content += $"Placements: {request.Placements.Count}\n";
        content += $"Include Registration Marks: {request.IncludeRegistrationMarks}\n";
        content += $"Include Crop Marks: {request.IncludeCropMarks}\n";
        content += $"Bleed Size: {request.BleedSize}mm\n";

        return await Task.FromResult(Encoding.UTF8.GetBytes(content));
    }

    public async Task<byte[]> GenerateReportAsync(ExportRequest request)
    {
        var sheet = await _sheetService.GetSheetAsync(request.SheetId);
        if (sheet == null)
            return Array.Empty<byte>();

        var csv = new StringBuilder();
        csv.AppendLine("Sheet Name,Width,Height,Die Line,X,Y,Rotation,Quantity");

        foreach (var placement in request.Placements)
        {
            var dieLine = await _dieLineService.GetDieLineAsync(placement.DieLineId);
            if (dieLine != null)
            {
                csv.AppendLine($"{sheet.Name},{sheet.Width},{sheet.Height}," +
                             $"{dieLine.FileName},{placement.X},{placement.Y}," +
                             $"{placement.Rotation},{placement.Quantity}");
            }
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }
}
