namespace Przypominajka.Models;

public class Powiadomienie
{
    public int IdPowiadomienia { get; set; }
    public int IdZadania { get; set; }
    public DateTime DataZaplanowania { get; set; }
    public DateTime? DataWyslania { get; set; }
    public bool StatusWyslania { get; set; } = false;
    public string TrescPowiadomienia { get; set; } = string.Empty;

    public Zadanie Zadanie { get; set; } = null!;
}
