using JobGanging.Core.Interfaces;
using JobGanging.Core.Models;

namespace JobGanging.Core.Services;

public class SheetService : ISheetService
{
    private readonly List<Sheet> _sheets = new();

    public async Task<Sheet> CreateSheetAsync(SheetRequest request)
    {
        var sheet = new Sheet
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Width = request.Width,
            Height = request.Height,
            MarginTop = request.MarginTop,
            MarginBottom = request.MarginBottom,
            MarginLeft = request.MarginLeft,
            MarginRight = request.MarginRight,
            Material = request.Material,
            CreatedAt = DateTime.UtcNow
        };

        _sheets.Add(sheet);
        return await Task.FromResult(sheet);
    }

    public async Task<List<Sheet>> GetAllSheetsAsync()
    {
        return await Task.FromResult(_sheets.ToList());
    }

    public async Task<Sheet?> GetSheetAsync(Guid id)
    {
        return await Task.FromResult(_sheets.FirstOrDefault(s => s.Id == id));
    }

    public async Task<Sheet> UpdateSheetAsync(Guid id, SheetRequest request)
    {
        var sheet = _sheets.FirstOrDefault(s => s.Id == id);
        if (sheet == null)
            throw new InvalidOperationException("Sheet not found");

        sheet.Name = request.Name;
        sheet.Width = request.Width;
        sheet.Height = request.Height;
        sheet.MarginTop = request.MarginTop;
        sheet.MarginBottom = request.MarginBottom;
        sheet.MarginLeft = request.MarginLeft;
        sheet.MarginRight = request.MarginRight;
        sheet.Material = request.Material;

        return await Task.FromResult(sheet);
    }

    public async Task DeleteSheetAsync(Guid id)
    {
        var sheet = _sheets.FirstOrDefault(s => s.Id == id);
        if (sheet != null)
        {
            _sheets.Remove(sheet);
        }
        await Task.CompletedTask;
    }
}
