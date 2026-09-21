using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteAdam.Data.Entity.CentrifugalPumps;
using WebAppEstimate.Areas.SiteAdam.Data.Entity.Pumps.CentrifugalPumpsCalculationHistory;

namespace WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;

public class AppDbContextCentrifugalPump : DbContext
{
    //---
    /*public AppDbContextCentrifugalPump()
    {
    }*/

    public AppDbContextCentrifugalPump(
        DbContextOptions<AppDbContextCentrifugalPump> options)
        : base(options)
    {
    }
    
    

    public DbSet<PumpSeries> PumpSeries { get; set; } = null!;
    public DbSet<PumpImpeller> PumpImpellers { get; set; } = null!;
    public DbSet<PumpWeight> PumpWeights { get; set; } = null!;
    public DbSet<PumpVolume> PumpVolumes { get; set; } = null!;

    public DbSet<PumpCentrifugalDesign> PumpCentrifugalDesigns { get; set; } = null!;

    //---для сохранения в полученых результатом после расчета 
    public DbSet<PumpCentrifugalHistory> PumpCentrifugalHistories { get; set; } = null!;
    //---


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //---
        // 1. Связь ОДИН-КО-МНОГИМ: Серия -> Рабочие колеса
        modelBuilder.Entity<PumpImpeller>()
            .HasOne(i => i.PumpSeries)
            .WithMany(s => s.Impeller)
            .HasForeignKey(i => i.PumpSeriesId)
            .OnDelete(DeleteBehavior.Cascade); // Если удалить серию, удалятся и её колеса

        // 2. Связь ОДИН-КО-МНОГИМ: Серия -> Вес
        modelBuilder.Entity<PumpWeight>()
            .HasOne(w => w.PumpSeries)
            .WithMany(s => s.Weight)
            .HasForeignKey(w => w.PumpSeriesId)
            .OnDelete(DeleteBehavior.Cascade); // Если удалить серию, удалится и её вес

        // 3. Связь ОДИН-КО-МНОГИМ: Серия -> Объем
        modelBuilder.Entity<PumpVolume>()
            .HasOne(v => v.PumpSeries)
            .WithMany(s => s.Volume)
            .HasForeignKey(v => v.PumpSeriesId)
            .OnDelete(DeleteBehavior.Cascade); // Если удалить серию, удалится и её объем


        // 4. НАСТРОЙКА СВЯЗЕЙ ДЛЯ ТАБЛИЦЫ СБОРКИ (PumpCentrifugalDesign)

        // Связь с Серией
        modelBuilder.Entity<PumpCentrifugalDesign>()
            .HasOne(d => d.PumpSeries)
            .WithMany() // У серии нет прямой коллекции дизайнов (и не нужно)
            .HasForeignKey(d => d.PumpSeriesId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь с Колесом (Отключаем каскад, так как колесо уже удалится вместе с Серией)
        modelBuilder.Entity<PumpCentrifugalDesign>()
            .HasOne(d => d.PumpImpeller)
            .WithMany()
            .HasForeignKey(d => d.PumpImpellerId)
            .OnDelete(DeleteBehavior.Restrict); // Защита от кругового удаления

        // Связь с Весом
        modelBuilder.Entity<PumpCentrifugalDesign>()
            .HasOne(d => d.PumpWeight)
            .WithMany()
            .HasForeignKey(d => d.PumpWeightId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь с Объемом
        modelBuilder.Entity<PumpCentrifugalDesign>()
            .HasOne(d => d.PumpVolume)
            .WithMany()
            .HasForeignKey(d => d.PumpVolumeId)
            .OnDelete(DeleteBehavior.Restrict);

        //---связь с Историей сохранения центробежных насосов
        modelBuilder.Entity<PumpCentrifugalHistory>()
            .HasOne(h => h.PumpSeries)
            .WithMany()
            .HasForeignKey(h => h.SelectedSeriesId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}