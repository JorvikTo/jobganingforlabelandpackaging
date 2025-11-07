using JobGanging.Core.Interfaces;
using JobGanging.Core.Models;

namespace JobGanging.Core.Services;

public class NestingService : INestingService
{
    private readonly IDieLineService _dieLineService;
    private readonly ISheetService _sheetService;

    public NestingService(IDieLineService dieLineService, ISheetService sheetService)
    {
        _dieLineService = dieLineService;
        _sheetService = sheetService;
    }

    public async Task<NestingResult> OptimizeLayoutAsync(NestingRequest request)
    {
        var sheet = await _sheetService.GetSheetAsync(request.SheetId);
        if (sheet == null)
            throw new InvalidOperationException("Sheet not found");

        var dieLines = new List<DieLine>();
        foreach (var dlq in request.DieLines)
        {
            var dieLine = await _dieLineService.GetDieLineAsync(dlq.DieLineId);
            if (dieLine != null)
            {
                for (int i = 0; i < dlq.Quantity; i++)
                {
                    dieLines.Add(dieLine);
                }
            }
        }

        // Implement simple nesting algorithm (First-Fit Decreasing)
        var placements = new List<PlacedDieLine>();
        var currentX = sheet.MarginLeft;
        var currentY = sheet.MarginTop;
        var rowHeight = 0.0;

        foreach (var dieLine in dieLines.OrderByDescending(d => d.Width * d.Height))
        {
            var width = dieLine.Width + request.Options.Spacing;
            var height = dieLine.Height + request.Options.Spacing;

            // Check if fits in current row
            if (currentX + width > sheet.Width - sheet.MarginRight)
            {
                // Move to next row
                currentX = sheet.MarginLeft;
                currentY += rowHeight + request.Options.Spacing;
                rowHeight = 0;
            }

            // Check if fits in sheet
            if (currentY + height <= sheet.Height - sheet.MarginBottom)
            {
                placements.Add(new PlacedDieLine
                {
                    Id = Guid.NewGuid(),
                    DieLineId = dieLine.Id,
                    X = currentX,
                    Y = currentY,
                    Rotation = 0,
                    Quantity = 1
                });

                currentX += width;
                rowHeight = Math.Max(rowHeight, height);
            }
        }

        // Calculate utilization
        var totalArea = sheet.Width * sheet.Height;
        var usedArea = dieLines.Sum(d => d.Width * d.Height);
        var utilization = (usedArea / totalArea) * 100;

        return new NestingResult
        {
            SheetId = request.SheetId,
            Placements = placements,
            Utilization = utilization,
            WastePercentage = 100 - utilization,
            TotalArea = totalArea,
            UsedArea = usedArea
        };
    }

    public async Task<NestingResult> ManualAdjustAsync(ManualAdjustmentRequest request)
    {
        // TODO: Implement manual adjustment with collision detection
        return await Task.FromResult(new NestingResult
        {
            SheetId = request.SheetId,
            Placements = new List<PlacedDieLine>()
        });
    }

    public async Task<double> CalculateWasteAsync(WasteCalculationRequest request)
    {
        var sheet = await _sheetService.GetSheetAsync(request.SheetId);
        if (sheet == null)
            return 0;

        var totalArea = sheet.Width * sheet.Height;
        var usedArea = 0.0;

        foreach (var placement in request.Placements)
        {
            var dieLine = await _dieLineService.GetDieLineAsync(placement.DieLineId);
            if (dieLine != null)
            {
                usedArea += dieLine.Width * dieLine.Height * placement.Quantity;
            }
        }

        return ((totalArea - usedArea) / totalArea) * 100;
    }
}
