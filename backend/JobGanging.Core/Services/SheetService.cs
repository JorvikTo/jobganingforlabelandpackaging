using JobGanging.Core.Interfaces;
using JobGanging.Core.Models;
using JobGanging.Data;
using Microsoft.EntityFrameworkCore;

namespace JobGanging.Core.Services;

public class SheetService : ISheetService
{
    private readonly ApplicationDbContext _dbContext;

    public SheetService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

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

        _dbContext.Sheets.Add(sheet);
        await _dbContext.SaveChangesAsync();

        return sheet;
    }

    public async Task<List<Sheet>> GetAllSheetsAsync()
    {
        return await _dbContext.Sheets
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<Sheet?> GetSheetAsync(Guid id)
    {
        return await _dbContext.Sheets
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Sheet> UpdateSheetAsync(Guid id, SheetRequest request)
    {
        var sheet = await _dbContext.Sheets.FindAsync(id);
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

        await _dbContext.SaveChangesAsync();

        return sheet;
    }

    public async Task DeleteSheetAsync(Guid id)
    {
        var sheet = await _dbContext.Sheets.FindAsync(id);
        if (sheet != null)
        {
            _dbContext.Sheets.Remove(sheet);
            await _dbContext.SaveChangesAsync();
        }
    }
}
