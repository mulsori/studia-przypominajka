using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Przypominajka.Data;
using Przypominajka.Models;
using Przypominajka.Views;
using System.Collections.ObjectModel;

namespace Przypominajka.ViewModels;

public partial class SerieViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public SerieViewModel(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [ObservableProperty]
    public partial ObservableCollection<Seria> Serie { get; set; } = [];

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [RelayCommand]
    public async Task WczytajAsync()
    {
        IsLoading = true;
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var lista = await db.Serie.OrderBy(s => s.InterwalPowtarzania).ToListAsync();
            Serie = new ObservableCollection<Seria>(lista);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task DodajAsync()
        => await Shell.Current.GoToAsync(nameof(SeriaFormPage));

    [RelayCommand]
    private async Task EdytujAsync(Seria seria)
    {
        var param = new ShellNavigationQueryParameters { { "IdSerii", seria.IdSerii } };
        await Shell.Current.GoToAsync(nameof(SeriaFormPage), param);
    }

    [RelayCommand]
    private async Task PrzepnijAktywnoscAsync(Seria seria)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var s = await db.Serie.FindAsync(seria.IdSerii);
        if (s is null) return;
        s.CzyAktywna = !s.CzyAktywna;
        await db.SaveChangesAsync();
        seria.CzyAktywna = s.CzyAktywna;
        // Wymusz odświeżenie widoku
        var idx = Serie.IndexOf(seria);
        if (idx >= 0) { Serie.RemoveAt(idx); Serie.Insert(idx, seria); }
    }

    [RelayCommand]
    private async Task UsunAsync(Seria seria)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        int liczba = await db.Zadania.CountAsync(z => z.IdSerii == seria.IdSerii);
        if (liczba > 0)
        {
            await Shell.Current.DisplayAlert("Nie można usunąć",
                $"Seria ma przypisane {liczba} zadań. Usuń najpierw zadania lub usuń przypisanie serii.", "OK");
            return;
        }

        bool ok = await Shell.Current.DisplayAlert("Usuwanie",
            $"Usunac serie: {seria.InterwalPowtarzania}?", "Tak, usun", "Anuluj");
        if (!ok) return;

        var s = await db.Serie.FindAsync(seria.IdSerii);
        if (s is null) return;
        db.Serie.Remove(s);
        await db.SaveChangesAsync();
        Serie.Remove(seria);
    }
}
