using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Przypominajka.Data;
using Przypominajka.Models;
using Przypominajka.Views;
using System.Collections.ObjectModel;

namespace Przypominajka.ViewModels;

public partial class ZadaniaViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public ZadaniaViewModel(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [ObservableProperty]
    public partial ObservableCollection<Zadanie> Zadania { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<Kategoria> Kategorie { get; set; } = [];

    [ObservableProperty]
    public partial Kategoria? WybranaKategoria { get; set; }

    [ObservableProperty]
    public partial string FiltrStatusu { get; set; } = "Wszystkie";

    [ObservableProperty]
    public partial string Szukaj { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    public List<string> StatusyFiltra { get; } = ["Wszystkie", "Do wykonania", "Wykonane"];

    [RelayCommand]
    public async Task WczytajAsync()
    {
        IsLoading = true;
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var kategorie = await db.Kategorie.OrderBy(k => k.NazwaKategorii).ToListAsync();
            Kategorie = new ObservableCollection<Kategoria>(kategorie);

            await OdswiezListeAsync(db);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task OdswiezListeAsync(AppDbContext db)
    {
        var query = db.Zadania
            .Include(z => z.Kategoria)
            .Include(z => z.Seria)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Szukaj))
            query = query.Where(z => z.NazwaZadania.Contains(Szukaj) ||
                                     (z.OpisZadania != null && z.OpisZadania.Contains(Szukaj)));

        if (WybranaKategoria != null)
            query = query.Where(z => z.IdKategorii == WybranaKategoria.IdKategorii);

        if (FiltrStatusu == "Do wykonania")
            query = query.Where(z => !z.StatusWykonania);
        else if (FiltrStatusu == "Wykonane")
            query = query.Where(z => z.StatusWykonania);

        var lista = await query.OrderByDescending(z => z.DataModyfikacji).ToListAsync();
        Zadania = new ObservableCollection<Zadanie>(lista);
    }

    [RelayCommand]
    private async Task ZastosujFiltryAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        await OdswiezListeAsync(db);
    }

    [RelayCommand]
    private void WyczyscFiltry()
    {
        WybranaKategoria = null;
        FiltrStatusu = "Wszystkie";
        Szukaj = string.Empty;
    }

    [RelayCommand]
    private async Task DodajAsync()
    {
        await Shell.Current.GoToAsync(nameof(ZadanieFormPage));
    }

    [RelayCommand]
    private async Task EdytujAsync(Zadanie zadanie)
    {
        var param = new ShellNavigationQueryParameters { { "IdZadania", zadanie.IdZadania } };
        await Shell.Current.GoToAsync(nameof(ZadanieFormPage), param);
    }

    [RelayCommand]
    private async Task PrzepnijStatusAsync(Zadanie zadanie)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var z = await db.Zadania.FindAsync(zadanie.IdZadania);
        if (z is null) return;

        z.StatusWykonania = !z.StatusWykonania;
        z.DataModyfikacji = DateTime.Now;
        await db.SaveChangesAsync();

        zadanie.StatusWykonania = z.StatusWykonania;
        zadanie.DataModyfikacji = z.DataModyfikacji;
        // Odśwież żeby lista posortowała
        await using var db2 = await _dbFactory.CreateDbContextAsync();
        await OdswiezListeAsync(db2);
    }

    [RelayCommand]
    private async Task UsunAsync(Zadanie zadanie)
    {
        bool potwierdzenie = await Shell.Current.DisplayAlert(
            "Usuwanie",
            $"Usunac zadanie: {zadanie.NazwaZadania}?",
            "Tak, usuń", "Anuluj");
        if (!potwierdzenie) return;

        await using var db = await _dbFactory.CreateDbContextAsync();
        var z = await db.Zadania.FindAsync(zadanie.IdZadania);
        if (z is null) return;

        db.Zadania.Remove(z);
        await db.SaveChangesAsync();
        Zadania.Remove(zadanie);
    }
}
