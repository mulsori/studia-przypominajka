namespace Przypominajka.Models;

public class Kategoria
{
    public int IdKategorii { get; set; }
    public string NazwaKategorii { get; set; } = string.Empty;
    public string KolorEtykiety { get; set; } = "#3498DB";

    public ICollection<Zadanie> Zadania { get; set; } = new List<Zadanie>();
}
