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
        // Check if sheet size optimization is enabled
        if (request.Options.OptimizeSheetSize)
        {
            return await OptimizeWithVariableSheetSizeAsync(request);
        }

        var sheet = await _sheetService.GetSheetAsync(request.SheetId);
        if (sheet == null)
            throw new InvalidOperationException("Sheet not found");

        return await OptimizeLayoutForSheetAsync(sheet, request);
    }

    private async Task<NestingResult> OptimizeWithVariableSheetSizeAsync(NestingRequest request)
    {
        // Get die lines to nest
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

        if (dieLines.Count == 0)
        {
            throw new InvalidOperationException("No die lines to optimize");
        }

        // Calculate total area needed
        var totalDieLineArea = dieLines.Sum(d => d.Width * d.Height);
        var maxDieLineWidth = dieLines.Max(d => d.Width);
        var maxDieLineHeight = dieLines.Max(d => d.Height);

        // Try different sheet sizes within the range
        NestingResult? bestResult = null;
        double bestUtilization = 0;

        // Generate candidate sheet sizes
        var candidateSizes = GenerateCandidateSheetSizes(
            request.Options.MinSheetWidth,
            request.Options.MaxSheetWidth,
            request.Options.MinSheetHeight,
            request.Options.MaxSheetHeight,
            maxDieLineWidth,
            maxDieLineHeight,
            totalDieLineArea
        );

        foreach (var (width, height) in candidateSizes)
        {
            // Create temporary sheet
            var tempSheet = new Sheet
            {
                Id = Guid.NewGuid(),
                Name = "Optimized Sheet",
                Width = width,
                Height = height,
                MarginTop = 10,
                MarginBottom = 10,
                MarginLeft = 10,
                MarginRight = 10,
                Material = "Auto",
                CreatedAt = DateTime.UtcNow
            };

            var result = await OptimizeLayoutForSheetAsync(tempSheet, request);

            // Check if all items fit
            if (result.Placements.Count == dieLines.Count && result.Utilization > bestUtilization)
            {
                bestUtilization = result.Utilization;
                bestResult = result;
                bestResult.OptimizedSheet = tempSheet;
                bestResult.IsOptimizedSize = true;
            }
        }

        if (bestResult == null)
        {
            throw new InvalidOperationException(
                $"Could not fit all die lines within sheet size range: " +
                $"{request.Options.MinSheetWidth}x{request.Options.MinSheetHeight} to " +
                $"{request.Options.MaxSheetWidth}x{request.Options.MaxSheetHeight}"
            );
        }

        return bestResult;
    }

    private List<(double width, double height)> GenerateCandidateSheetSizes(
        double minWidth, double maxWidth, double minHeight, double maxHeight,
        double maxDieWidth, double maxDieHeight, double totalArea)
    {
        var candidates = new List<(double width, double height)>();

        // Ensure minimum size can fit at least one die line
        minWidth = Math.Max(minWidth, maxDieWidth + 20); // Add margin
        minHeight = Math.Max(minHeight, maxDieHeight + 20);

        // Generate sizes at intervals
        var widthStep = (maxWidth - minWidth) / 5;
        var heightStep = (maxHeight - minHeight) / 5;

        for (double w = minWidth; w <= maxWidth; w += widthStep)
        {
            for (double h = minHeight; h <= maxHeight; h += heightStep)
            {
                candidates.Add((w, h));
            }
        }

        // Add corner cases
        candidates.Add((minWidth, minHeight));
        candidates.Add((maxWidth, maxHeight));
        candidates.Add((minWidth, maxHeight));
        candidates.Add((maxWidth, minHeight));

        // Sort by area (try smaller sheets first for better utilization)
        return candidates.OrderBy(c => c.width * c.height).ToList();
    }

    private async Task<NestingResult> OptimizeLayoutForSheetAsync(Sheet sheet, NestingRequest request)
    {
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
            SheetId = sheet.Id,
            Placements = placements,
            Utilization = utilization,
            WastePercentage = 100 - utilization,
            TotalArea = totalArea,
            UsedArea = usedArea,
            IsOptimizedSize = false
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
