using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Przypominajka.Data;
using Przypominajka.Models;
using System.Collections.ObjectModel;

namespace Przypominajka.ViewModels;

public partial class PowiadomieniaViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public PowiadomieniaViewModel(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [ObservableProperty]
    public partial ObservableCollection<Powiadomienie> Powiadomienia { get; set; } = [];

    [ObservableProperty]
    public partial string FiltrStatusu { get; set; } = "Wszystkie";

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    public List<string> StatusyFiltra { get; } = ["Wszystkie", "Oczekujące", "Wysłane"];

    [RelayCommand]
    public async Task WczytajAsync()
    {
        IsLoading = true;
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            await OdswiezAsync(db);
        }
        finally { IsLoading = false; }
    }

    private async Task OdswiezAsync(AppDbContext db)
    {
        var query = db.Powiadomienia
            .Include(p => p.Zadanie)
            .AsQueryable();

        if (FiltrStatusu == "Oczekujące")
            query = query.Where(p => !p.StatusWyslania);
        else if (FiltrStatusu == "Wysłane")
            query = query.Where(p => p.StatusWyslania);

        var lista = await query.OrderByDescending(p => p.DataZaplanowania).ToListAsync();
        Powiadomienia = new ObservableCollection<Powiadomienie>(lista);
    }

    [RelayCommand]
    private async Task ZastosujFiltryAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        await OdswiezAsync(db);
    }

    [RelayCommand]
    private async Task OznaczJakoWyslaneAsync(Powiadomienie p)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var pow = await db.Powiadomienia.FindAsync(p.IdPowiadomienia);
        if (pow is null) return;
        pow.StatusWyslania = true;
        pow.DataWyslania = DateTime.Now;
        await db.SaveChangesAsync();
        await OdswiezAsync(db);
    }

    [RelayCommand]
    private async Task UsunAsync(Powiadomienie p)
    {
        bool ok = await Shell.Current.DisplayAlert("Usuwanie",
            $"Usunąć to powiadomienie?", "Tak", "Anuluj");
        if (!ok) return;

        await using var db = await _dbFactory.CreateDbContextAsync();
        var pow = await db.Powiadomienia.FindAsync(p.IdPowiadomienia);
        if (pow is null) return;
        db.Powiadomienia.Remove(pow);
        await db.SaveChangesAsync();
        Powiadomienia.Remove(p);
    }

    [RelayCommand]
    private async Task DodajPowiadomienieDlaZadaniaAsync()
    {
        // Prosta wersja: dodaje powiadomienie z datą teraz+5min dla pierwszego niezakończonego zadania
        // W praktyce możesz rozbudować o dialog wyboru zadania
        await using var db = await _dbFactory.CreateDbContextAsync();
        var zadanie = await db.Zadania.FirstOrDefaultAsync(z => !z.StatusWykonania);
        if (zadanie is null)
        {
            await Shell.Current.DisplayAlert("Brak zadań", "Nie ma aktywnych zadań do przypisania.", "OK");
            return;
        }

        db.Powiadomienia.Add(new Powiadomienie
        {
            IdZadania = zadanie.IdZadania,
            DataZaplanowania = DateTime.Now.AddMinutes(5),
            TrescPowiadomienia = $"Przypomnienie: {zadanie.NazwaZadania}",
            StatusWyslania = false
        });
        await db.SaveChangesAsync();
        await OdswiezAsync(db);
    }
}
