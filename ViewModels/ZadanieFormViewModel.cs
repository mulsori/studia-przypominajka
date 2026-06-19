using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Przypominajka.Data;
using Przypominajka.Models;
using System.Collections.ObjectModel;

namespace Przypominajka.ViewModels;

public partial class ZadanieFormViewModel : ObservableObject, IQueryAttributable
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private int _idZadania;

    public ZadanieFormViewModel(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    // ── Właściwości formularza ──────────────────────────────────────────────────

    [ObservableProperty]
    public partial string Tytul { get; set; } = "Nowe zadanie";

    [ObservableProperty]
    public partial string NazwaZadania { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string OpisZadania { get; set; } = string.Empty;

    [ObservableProperty]
    public partial DateTime TerminWykonania { get; set; } = DateTime.Now.AddDays(1);

    [ObservableProperty]
    public partial TimeSpan TerminWykonaniaCzas { get; set; } = TimeSpan.FromHours(9);

    [ObservableProperty]
    public partial DateTime TerminPrzypomnienia { get; set; } = DateTime.Now.AddDays(1);

    [ObservableProperty]
    public partial TimeSpan TerminPrzypomnieniaCzas { get; set; } = TimeSpan.FromHours(8);

    [ObservableProperty]
    public partial bool StatusWykonania { get; set; } = false;

    [ObservableProperty]
    public partial bool CzyAktywnePrzypomnienie { get; set; } = true;

    [ObservableProperty]
    public partial int InterwalPrzypominaniaPoTerminie { get; set; } = 60;

    [ObservableProperty]
    public partial Kategoria? WybranaKategoria { get; set; }

    [ObservableProperty]
    public partial Seria? WybranaSeria { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Kategoria> Kategorie { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<Seria> Serie { get; set; } = [];

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    public List<string> InterwalePowtarzania { get; } = ["codziennie", "co tydzień", "co miesiąc"];

    // ── Inicjalizacja (wywoływana przy nawigacji) ───────────────────────────────

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("IdZadania", out var val) && val is int id)
            _idZadania = id;
    }

    [RelayCommand]
    public async Task WczytajAsync()
    {
        IsLoading = true;
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var kategorie = await db.Kategorie.OrderBy(k => k.NazwaKategorii).ToListAsync();
            Kategorie = new ObservableCollection<Kategoria>(kategorie);
            Kategorie.Insert(0, new Kategoria { IdKategorii = 0, NazwaKategorii = "(brak kategorii)", KolorEtykiety = "#CCCCCC" });

            var serie = await db.Serie.Where(s => s.CzyAktywna).OrderBy(s => s.InterwalPowtarzania).ToListAsync();
            Serie = new ObservableCollection<Seria>(serie);
            Serie.Insert(0, new Seria { IdSerii = 0, InterwalPowtarzania = "(jednorazowe)" });

            if (_idZadania > 0)
            {
                Tytul = "Edytuj zadanie";
                var z = await db.Zadania
                    .Include(x => x.Kategoria)
                    .Include(x => x.Seria)
                    .FirstOrDefaultAsync(x => x.IdZadania == _idZadania);
                if (z is not null)
                {
                    NazwaZadania = z.NazwaZadania;
                    OpisZadania = z.OpisZadania ?? string.Empty;
                    TerminWykonania = z.TerminWykonania.Date;
                    TerminWykonaniaCzas = z.TerminWykonania.TimeOfDay;
                    TerminPrzypomnienia = z.TerminPrzypomnienia.Date;
                    TerminPrzypomnieniaCzas = z.TerminPrzypomnienia.TimeOfDay;
                    StatusWykonania = z.StatusWykonania;
                    CzyAktywnePrzypomnienie = z.CzyAktywnePrzypomnienie;
                    InterwalPrzypominaniaPoTerminie = z.InterwalPrzypominaniaPoTerminie;
                    WybranaKategoria = z.IdKategorii.HasValue
                        ? Kategorie.FirstOrDefault(k => k.IdKategorii == z.IdKategorii)
                        : Kategorie[0];
                    WybranaSeria = z.IdSerii.HasValue
                        ? Serie.FirstOrDefault(s => s.IdSerii == z.IdSerii)
                        : Serie[0];
                }
            }
            else
            {
                WybranaKategoria = Kategorie[0];
                WybranaSeria = Serie[0];
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ZapiszAsync()
    {
        if (string.IsNullOrWhiteSpace(NazwaZadania))
        {
            await Shell.Current.DisplayAlert("Błąd", "Nazwa zadania jest wymagana.", "OK");
            return;
        }
        if (InterwalPrzypominaniaPoTerminie <= 0)
        {
            await Shell.Current.DisplayAlert("Błąd", "Interwał przypominania musi być większy od 0.", "OK");
            return;
        }

        var terminWykonania = TerminWykonania.Date + TerminWykonaniaCzas;
        var terminPrzypomnienia = TerminPrzypomnienia.Date + TerminPrzypomnieniaCzas;

        if (terminPrzypomnienia > terminWykonania)
        {
            await Shell.Current.DisplayAlert("Błąd", "Termin przypomnienia musi być ≤ terminowi wykonania.", "OK");
            return;
        }

        await using var db = await _dbFactory.CreateDbContextAsync();

        if (_idZadania > 0)
        {
            var z = await db.Zadania.FindAsync(_idZadania);
            if (z is null) return;
            UzupelnijModel(z, terminWykonania, terminPrzypomnienia);
            z.DataModyfikacji = DateTime.Now;
        }
        else
        {
            var nowe = new Zadanie { DataUtworzenia = DateTime.Now, DataModyfikacji = DateTime.Now };
            UzupelnijModel(nowe, terminWykonania, terminPrzypomnienia);
            db.Zadania.Add(nowe);
        }

        await db.SaveChangesAsync();
        await Shell.Current.GoToAsync("..");
    }

    private void UzupelnijModel(Zadanie z, DateTime terminWykonania, DateTime terminPrzypomnienia)
    {
        z.NazwaZadania = NazwaZadania.Trim();
        z.OpisZadania = string.IsNullOrWhiteSpace(OpisZadania) ? null : OpisZadania.Trim();
        z.TerminWykonania = terminWykonania;
        z.TerminPrzypomnienia = terminPrzypomnienia;
        z.StatusWykonania = StatusWykonania;
        z.CzyAktywnePrzypomnienie = CzyAktywnePrzypomnienie;
        z.InterwalPrzypominaniaPoTerminie = InterwalPrzypominaniaPoTerminie;
        z.IdKategorii = WybranaKategoria?.IdKategorii > 0 ? WybranaKategoria.IdKategorii : null;
        z.IdSerii = WybranaSeria?.IdSerii > 0 ? WybranaSeria.IdSerii : null;
    }

    [RelayCommand]
    private async Task AnulujAsync() => await Shell.Current.GoToAsync("..");
}
