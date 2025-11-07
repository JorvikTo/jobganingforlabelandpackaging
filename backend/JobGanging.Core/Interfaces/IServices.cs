using JobGanging.Core.Models;

namespace JobGanging.Core.Interfaces;

public interface IDieLineService
{
    Task<DieLine> ImportDieLineAsync(Stream fileStream, string fileExtension, string fileName);
    Task<List<DieLine>> GetAllDieLinesAsync();
    Task<DieLine?> GetDieLineAsync(Guid id);
    Task DeleteDieLineAsync(Guid id);
}

public interface INestingService
{
    Task<NestingResult> OptimizeLayoutAsync(NestingRequest request);
    Task<NestingResult> ManualAdjustAsync(ManualAdjustmentRequest request);
    Task<double> CalculateWasteAsync(WasteCalculationRequest request);
}

public interface ISheetService
{
    Task<Sheet> CreateSheetAsync(SheetRequest request);
    Task<List<Sheet>> GetAllSheetsAsync();
    Task<Sheet?> GetSheetAsync(Guid id);
    Task<Sheet> UpdateSheetAsync(Guid id, SheetRequest request);
    Task DeleteSheetAsync(Guid id);
}

public interface IExportService
{
    Task<byte[]> ExportToPdfAsync(ExportRequest request);
    Task<byte[]> GenerateReportAsync(ExportRequest request);
}
