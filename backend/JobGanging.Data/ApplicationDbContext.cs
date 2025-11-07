using System.Linq;
using System.Text.Json;
using JobGanging.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace JobGanging.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DieLine> DieLines { get; set; }
    public DbSet<Sheet> Sheets { get; set; }
    public DbSet<Job> Jobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DieLine>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<Sheet>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<Job>()
            .HasKey(j => j.Id);

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var pointComparer = new ValueComparer<List<Point>>(
            (c1, c2) => JsonSerializer.Serialize(c1, jsonOptions) == JsonSerializer.Serialize(c2, jsonOptions),
            c => c.Aggregate(0, (hash, point) => HashCode.Combine(hash, point.X.GetHashCode(), point.Y.GetHashCode())),
            c => JsonSerializer.Deserialize<List<Point>>(JsonSerializer.Serialize(c, jsonOptions), jsonOptions) ?? new List<Point>());

        var spotColorComparer = new ValueComparer<List<SpotColor>>(
            (c1, c2) => JsonSerializer.Serialize(c1, jsonOptions) == JsonSerializer.Serialize(c2, jsonOptions),
            c => c.Aggregate(0, (hash, color) => HashCode.Combine(hash, color.Name?.GetHashCode() ?? 0, color.ColorValue?.GetHashCode() ?? 0)),
            c => JsonSerializer.Deserialize<List<SpotColor>>(JsonSerializer.Serialize(c, jsonOptions), jsonOptions) ?? new List<SpotColor>());

        modelBuilder.Entity<DieLine>()
            .Property(d => d.Outline)
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => string.IsNullOrEmpty(v) ? new List<Point>() : JsonSerializer.Deserialize<List<Point>>(v, jsonOptions) ?? new List<Point>())
            .Metadata.SetValueComparer(pointComparer);

        modelBuilder.Entity<DieLine>()
            .Property(d => d.SpotColors)
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => string.IsNullOrEmpty(v) ? new List<SpotColor>() : JsonSerializer.Deserialize<List<SpotColor>>(v, jsonOptions) ?? new List<SpotColor>())
            .Metadata.SetValueComparer(spotColorComparer);
    }
}
