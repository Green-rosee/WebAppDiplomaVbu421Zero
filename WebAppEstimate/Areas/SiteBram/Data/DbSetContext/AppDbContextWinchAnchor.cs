using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

namespace WebAppEstimate.Areas.SiteBram.Data.DbSetContext;

public class AppDbContextWinchAnchor : DbContext
{
    /*public AppDbContextWinchAnchor()
    {
    }*/

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

        //---для реализации множества таблиц TPC
        modelBuilder.Entity<AWinchBase>()
            .UseTpcMappingStrategy();
        
        modelBuilder.Entity<AWinchBase>()
            .Property(x => x.Id)
            .ValueGeneratedNever();
        //----
        
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
        
        modelBuilder.Entity<WinchWeight>()
            .HasOne(w => w.WinchAnchorSeries)
            .WithMany(s => s.Weights)
            .HasForeignKey(w => w.WinchAnchorSeriesId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WinchShaft>()
            .HasOne(s => s.WinchAnchorSeries)
            .WithMany(x => x.Shafts)
            .HasForeignKey(s => s.WinchAnchorSeriesId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WinchChain>()
            .HasOne(c => c.WinchAnchorSeries)
            .WithMany(s => s.Chains)
            .HasForeignKey(c => c.WinchAnchorSeriesId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}