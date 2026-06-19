using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Przypominajka.Data;
using Przypominajka.Models;

namespace Przypominajka.ViewModels;

public partial class KategoriaFormViewModel : ObservableObject, IQueryAttributable
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private int _idKategorii;

    public KategoriaFormViewModel(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [ObservableProperty]
    public partial string Tytul { get; set; } = "Nowa kategoria";

    [ObservableProperty]
    public partial string NazwaKategorii { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string KolorEtykiety { get; set; } = "#3498DB";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("IdKategorii", out var val) && val is int id)
            _idKategorii = id;
    }

    [RelayCommand]
    public async Task WczytajAsync()
    {
        if (_idKategorii <= 0) return;
        Tytul = "Edytuj kategorię";
        await using var db = await _dbFactory.CreateDbContextAsync();
        var k = await db.Kategorie.FindAsync(_idKategorii);
        if (k is null) return;
        NazwaKategorii = k.NazwaKategorii;
        KolorEtykiety = k.KolorEtykiety;
    }

    [RelayCommand]
    private async Task ZapiszAsync()
    {
        if (string.IsNullOrWhiteSpace(NazwaKategorii))
        {
            await Shell.Current.DisplayAlert("Błąd", "Nazwa kategorii jest wymagana.", "OK");
            return;
        }
        if (!KolorEtykiety.StartsWith('#') || KolorEtykiety.Length < 4)
        {
            await Shell.Current.DisplayAlert("Błąd", "Kolor musi być w formacie hex np. #3498DB", "OK");
            return;
        }

        await using var db = await _dbFactory.CreateDbContextAsync();

        if (_idKategorii > 0)
        {
            var k = await db.Kategorie.FindAsync(_idKategorii);
            if (k is null) return;
            k.NazwaKategorii = NazwaKategorii.Trim();
            k.KolorEtykiety = KolorEtykiety.Trim();
        }
        else
        {
            db.Kategorie.Add(new Kategoria
            {
                NazwaKategorii = NazwaKategorii.Trim(),
                KolorEtykiety = KolorEtykiety.Trim()
            });
        }

        try
        {
            await db.SaveChangesAsync();
            await Shell.Current.GoToAsync("..");
        }
        catch (DbUpdateException)
        {
            await Shell.Current.DisplayAlert("Błąd", "Kategoria o tej nazwie już istnieje.", "OK");
        }
    }

    [RelayCommand]
    private async Task AnulujAsync() => await Shell.Current.GoToAsync("..");
}
