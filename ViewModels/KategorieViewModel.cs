using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Przypominajka.Data;
using Przypominajka.Models;
using Przypominajka.Views;
using System.Collections.ObjectModel;

namespace Przypominajka.ViewModels;

public partial class KategorieViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public KategorieViewModel(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [ObservableProperty]
    public partial ObservableCollection<Kategoria> Kategorie { get; set; } = [];

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [RelayCommand]
    public async Task WczytajAsync()
    {
        IsLoading = true;
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var lista = await db.Kategorie.OrderBy(k => k.NazwaKategorii).ToListAsync();
            Kategorie = new ObservableCollection<Kategoria>(lista);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task DodajAsync()
        => await Shell.Current.GoToAsync(nameof(KategoriaFormPage));

    [RelayCommand]
    private async Task EdytujAsync(Kategoria kategoria)
    {
        var param = new ShellNavigationQueryParameters { { "IdKategorii", kategoria.IdKategorii } };
        await Shell.Current.GoToAsync(nameof(KategoriaFormPage), param);
    }

    [RelayCommand]
    private async Task UsunAsync(Kategoria kategoria)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        int liczbaZadan = await db.Zadania.CountAsync(z => z.IdKategorii == kategoria.IdKategorii);

        string komunikat = liczbaZadan > 0
            ? $"Kategoria ma {liczbaZadan} zadań. Po usunięciu kategorii zadania stracą przypisanie. Kontynuować?"
            : $"Usunac kategorie: {kategoria.NazwaKategorii}?";

        bool ok = await Shell.Current.DisplayAlert("Usuwanie", komunikat, "Tak, usuń", "Anuluj");
        if (!ok) return;

        var k = await db.Kategorie.FindAsync(kategoria.IdKategorii);
        if (k is null) return;
        db.Kategorie.Remove(k);
        await db.SaveChangesAsync();
        Kategorie.Remove(kategoria);
    }
}
