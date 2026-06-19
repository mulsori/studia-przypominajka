using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Przypominajka.Data;
using Przypominajka.ViewModels;
using Przypominajka.Views;

namespace Przypominajka;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── EF Core (fabryka pozwala tworzyć krótkotrwałe konteksty w VM) ──────
        builder.Services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlServer(
                "Server=localhost\\MSSQLSERVER01;Database=Przypominajka;Trusted_Connection=True;TrustServerCertificate=True;")
#if DEBUG
            .EnableSensitiveDataLogging()
#endif
        );

        // ── ViewModels ─────────────────────────────────────────────────────────
        builder.Services.AddTransient<ZadaniaViewModel>();
        builder.Services.AddTransient<ZadanieFormViewModel>();
        builder.Services.AddTransient<KategorieViewModel>();
        builder.Services.AddTransient<KategoriaFormViewModel>();
        builder.Services.AddTransient<SerieViewModel>();
        builder.Services.AddTransient<SeriaFormViewModel>();
        builder.Services.AddTransient<PowiadomieniaViewModel>();

        // ── Pages ──────────────────────────────────────────────────────────────
        builder.Services.AddTransient<ZadaniaPage>();
        builder.Services.AddTransient<ZadanieFormPage>();
        builder.Services.AddTransient<KategoriePage>();
        builder.Services.AddTransient<KategoriaFormPage>();
        builder.Services.AddTransient<SeriePage>();
        builder.Services.AddTransient<SeriaFormPage>();
        builder.Services.AddTransient<PowiadomieniaPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
