namespace Przypominajka.Models;

public class Zadanie
{
    public int IdZadania { get; set; }
    public int? IdKategorii { get; set; }
    public int? IdSerii { get; set; }
    public string NazwaZadania { get; set; } = string.Empty;
    public string? OpisZadania { get; set; }
    public DateTime TerminWykonania { get; set; } = DateTime.Now.AddDays(1);
    public DateTime TerminPrzypomnienia { get; set; } = DateTime.Now.AddHours(23);
    public bool StatusWykonania { get; set; } = false;
    public bool CzyAktywnePrzypomnienie { get; set; } = true;
    public int InterwalPrzypominaniaPoTerminie { get; set; } = 60;
    public DateTime DataUtworzenia { get; set; } = DateTime.Now;
    public DateTime DataModyfikacji { get; set; } = DateTime.Now;

    public Kategoria? Kategoria { get; set; }
    public Seria? Seria { get; set; }
    public ICollection<Powiadomienie> Powiadomienia { get; set; } = new List<Powiadomienie>();
}
