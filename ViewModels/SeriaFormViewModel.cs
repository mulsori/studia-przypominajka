using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Przypominajka.Data;
using Przypominajka.Models;

namespace Przypominajka.ViewModels;

public partial class SeriaFormViewModel : ObservableObject, IQueryAttributable
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private int _idSerii;

    public SeriaFormViewModel(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [ObservableProperty]
    public partial string Tytul { get; set; } = "Nowa seria";

    [ObservableProperty]
    public partial string InterwalPowtarzania { get; set; } = "codziennie";

    [ObservableProperty]
    public partial bool CzyAktywna { get; set; } = true;

    public List<string> DostepneInterwaly { get; } = ["codziennie", "co tydzień", "co miesiąc"];

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("IdSerii", out var val) && val is int id)
            _idSerii = id;
    }

    [RelayCommand]
    public async Task WczytajAsync()
    {
        if (_idSerii <= 0) return;
        Tytul = "Edytuj serię";
        await using var db = await _dbFactory.CreateDbContextAsync();
        var s = await db.Serie.FindAsync(_idSerii);
        if (s is null) return;
        InterwalPowtarzania = s.InterwalPowtarzania;
        CzyAktywna = s.CzyAktywna;
    }

    [RelayCommand]
    private async Task ZapiszAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        if (_idSerii > 0)
        {
            var s = await db.Serie.FindAsync(_idSerii);
            if (s is null) return;
            s.InterwalPowtarzania = InterwalPowtarzania;
            s.CzyAktywna = CzyAktywna;
        }
        else
        {
            db.Serie.Add(new Seria
            {
                InterwalPowtarzania = InterwalPowtarzania,
                CzyAktywna = CzyAktywna
            });
        }

        await db.SaveChangesAsync();
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task AnulujAsync() => await Shell.Current.GoToAsync("..");
}
