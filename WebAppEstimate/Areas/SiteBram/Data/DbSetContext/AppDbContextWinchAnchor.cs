using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

namespace WebAppEstimate.Areas.SiteBram.Data.DbSetContext;

public class AppDbContextWinchAnchor : DbContext
{
    public AppDbContextWinchAnchor()
    {
    }

    public AppDbContextWinchAnchor(DbContextOptions<AppDbContextWinchAnchor> options) : base(options)
    {
    }

    public DbSet<WinchAnchorSeries> WinchAnchorSeries { get; set; } = null!;
    public DbSet<WinchWeight> WinchWeights { get; set; } = null!;
    public DbSet<WinchChain> WinchChains { get; set; } = null!;
    public DbSet<WinchShaft> WinchShafts { get; set; } = null!;

    public DbSet<WinchAnchorDesign> WinchAnchorDesigns { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WinchAnchorDesign>()
            .HasOne(x => x.WinchAnchorSeries)
            .WithMany()
            .HasForeignKey(x => x.WinchAnchorSeriesId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WinchAnchorDesign>()
            .HasOne(x => x.WinchWeight)
            .WithMany()
            .HasForeignKey(x => x.WinchWeightId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WinchAnchorDesign>()
            .HasOne(x => x.WinchShaft)
            .WithMany()
            .HasForeignKey(x => x.WinchShaftId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WinchAnchorDesign>()
            .HasOne(x => x.WinchChain)
            .WithMany()
            .HasForeignKey(x => x.WinchChainId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}