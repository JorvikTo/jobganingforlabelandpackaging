namespace JobGanging.Core.Models;

public class DieLine
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty; // PDF or DXF
    public double Width { get; set; }
    public double Height { get; set; }
    public List<Point> Outline { get; set; } = new();
    public List<SpotColor> SpotColors { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string RawData { get; set; } = string.Empty; // Base64 encoded file data
    public Guid? JobId { get; set; } // Link to parent job if extracted from artwork
    public bool HasArtwork { get; set; } // True if die line was extracted from artwork PDF
    public string? ArtworkData { get; set; } // Base64 encoded artwork (without die line)
}

public class Job
{
    public Guid Id { get; set; }
    public string JobName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string ArtworkData { get; set; } = string.Empty; // Full artwork PDF with die lines
    public Guid? ExtractedDieLineId { get; set; } // Die line extracted from artwork
    public string DieLineSpotColorName { get; set; } = string.Empty; // Spot color used for die line
    public int Quantity { get; set; } = 1;
}

public class Point
{
    public double X { get; set; }
    public double Y { get; set; }
}

public class SpotColor
{
    public string Name { get; set; } = string.Empty;
    public string ColorValue { get; set; } = string.Empty;
}

public class Sheet
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Width { get; set; }
    public double Height { get; set; }
    public double MarginTop { get; set; }
    public double MarginBottom { get; set; }
    public double MarginLeft { get; set; }
    public double MarginRight { get; set; }
    public string Material { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PlacedDieLine
{
    public Guid Id { get; set; }
    public Guid DieLineId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Rotation { get; set; } // 0, 90, 180, 270
    public int Quantity { get; set; }
}

public class NestingResult
{
    public Guid SheetId { get; set; }
    public List<PlacedDieLine> Placements { get; set; } = new();
    public double Utilization { get; set; } // Percentage
    public double WastePercentage { get; set; }
    public double TotalArea { get; set; }
    public double UsedArea { get; set; }
}

public class NestingRequest
{
    public Guid SheetId { get; set; }
    public List<DieLineQuantity> DieLines { get; set; } = new();
    public NestingOptions Options { get; set; } = new();
}

public class DieLineQuantity
{
    public Guid DieLineId { get; set; }
    public int Quantity { get; set; }
}

public class NestingOptions
{
    public double Spacing { get; set; } = 5.0; // mm
    public bool AllowRotation { get; set; } = true;
    public List<double> AllowedRotations { get; set; } = new() { 0, 90, 180, 270 };
    public int MaxIterations { get; set; } = 1000;
}

public class ManualAdjustmentRequest
{
    public Guid SheetId { get; set; }
    public Guid PlacementId { get; set; }
    public double NewX { get; set; }
    public double NewY { get; set; }
    public double NewRotation { get; set; }
}

public class WasteCalculationRequest
{
    public Guid SheetId { get; set; }
    public List<PlacedDieLine> Placements { get; set; } = new();
}

public class SheetRequest
{
    public string Name { get; set; } = string.Empty;
    public double Width { get; set; }
    public double Height { get; set; }
    public double MarginTop { get; set; }
    public double MarginBottom { get; set; }
    public double MarginLeft { get; set; }
    public double MarginRight { get; set; }
    public string Material { get; set; } = string.Empty;
}

public class ExportRequest
{
    public Guid SheetId { get; set; }
    public List<PlacedDieLine> Placements { get; set; } = new();
    public bool IncludeRegistrationMarks { get; set; } = true;
    public bool IncludeCropMarks { get; set; } = true;
    public double BleedSize { get; set; } = 3.0; // mm
}

public class JobImportRequest
{
    public string JobName { get; set; } = string.Empty;
    public string DieLineSpotColorName { get; set; } = "CutContour"; // Default spot color name for die lines
    public int Quantity { get; set; } = 1;
}

public class JobImportResult
{
    public Guid JobId { get; set; }
    public Guid DieLineId { get; set; }
    public string JobName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> DetectedSpotColors { get; set; } = new();
}
