using JobGanging.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace JobGanging.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DieLine> DieLines { get; set; }
    public DbSet<Sheet> Sheets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DieLine>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<Sheet>()
            .HasKey(s => s.Id);
    }
}
