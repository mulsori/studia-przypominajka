using Microsoft.EntityFrameworkCore;
using Przypominajka.Models;

namespace Przypominajka.Data;

public class AppDbContext : DbContext
{
    // ── Zmień connection string jeśli nie używasz LocalDB ──────────────────────
    // LocalDB:          "Server=(localdb)\\MSSQLLocalDB;Database=Przypominajka;Trusted_Connection=True;TrustServerCertificate=True;"
    // SQL Server Express: "Server=.\\SQLEXPRESS;Database=Przypominajka;Trusted_Connection=True;TrustServerCertificate=True;"
    // SQL Server lokalny: "Server=localhost;Database=Przypominajka;Trusted_Connection=True;TrustServerCertificate=True;"
    private const string ConnectionString =
        "Server=localhost\\MSSQLSERVER01;Database=Przypominajka;Trusted_Connection=True;TrustServerCertificate=True;";

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Kategoria> Kategorie { get; set; }
    public DbSet<Seria> Serie { get; set; }
    public DbSet<Zadanie> Zadania { get; set; }
    public DbSet<Powiadomienie> Powiadomienia { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Kategorie ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Kategoria>(e =>
        {
            e.ToTable("Kategorie");
            e.HasKey(k => k.IdKategorii);
            e.Property(k => k.IdKategorii).UseIdentityColumn();
            e.Property(k => k.NazwaKategorii).HasMaxLength(50).IsRequired();
            e.Property(k => k.KolorEtykiety).HasMaxLength(7).IsRequired();
            e.HasIndex(k => k.NazwaKategorii).IsUnique();
        });

        // ── Serie ──────────────────────────────────────────────────────────────
        modelBuilder.Entity<Seria>(e =>
        {
            e.ToTable("Serie");
            e.HasKey(s => s.IdSerii);
            e.Property(s => s.IdSerii).UseIdentityColumn();
            e.Property(s => s.InterwalPowtarzania).HasMaxLength(20).IsRequired();
            e.Property(s => s.CzyAktywna).HasDefaultValue(true);
        });

        // ── Zadania ────────────────────────────────────────────────────────────
        modelBuilder.Entity<Zadanie>(e =>
        {
            e.ToTable("Zadania");
            e.HasKey(z => z.IdZadania);
            e.Property(z => z.IdZadania).UseIdentityColumn();
            e.Property(z => z.NazwaZadania).HasMaxLength(100).IsRequired();
            e.Property(z => z.OpisZadania).HasColumnType("nvarchar(max)");
            e.Property(z => z.TerminWykonania).HasColumnType("datetime2(0)").IsRequired();
            e.Property(z => z.TerminPrzypomnienia).HasColumnType("datetime2(0)").IsRequired();
            e.Property(z => z.StatusWykonania).HasDefaultValue(false);
            e.Property(z => z.CzyAktywnePrzypomnienie).HasDefaultValue(true);
            e.Property(z => z.InterwalPrzypominaniaPoTerminie).IsRequired();
            e.Property(z => z.DataUtworzenia).HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSDATETIME()");
            e.Property(z => z.DataModyfikacji).HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSDATETIME()");

            // FK → Kategorie (SET NULL przy usunięciu)
            e.HasOne(z => z.Kategoria)
             .WithMany(k => k.Zadania)
             .HasForeignKey(z => z.IdKategorii)
             .OnDelete(DeleteBehavior.SetNull);

            // FK → Serie (NO ACTION)
            e.HasOne(z => z.Seria)
             .WithMany(s => s.Zadania)
             .HasForeignKey(z => z.IdSerii)
             .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Powiadomienia ──────────────────────────────────────────────────────
        modelBuilder.Entity<Powiadomienie>(e =>
        {
            e.ToTable("Powiadomienia");
            e.HasKey(p => p.IdPowiadomienia);
            e.Property(p => p.IdPowiadomienia).UseIdentityColumn();
            e.Property(p => p.DataZaplanowania).HasColumnType("datetime2(0)").IsRequired();
            e.Property(p => p.DataWyslania).HasColumnType("datetime2(0)");
            e.Property(p => p.StatusWyslania).HasDefaultValue(false);
            e.Property(p => p.TrescPowiadomienia).HasColumnType("nvarchar(max)").IsRequired();

            // FK → Zadania (CASCADE)
            e.HasOne(p => p.Zadanie)
             .WithMany(z => z.Powiadomienia)
             .HasForeignKey(p => p.IdZadania)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
