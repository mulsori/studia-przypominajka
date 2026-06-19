namespace Przypominajka.Models;

public class Seria
{
    public int IdSerii { get; set; }
    public string InterwalPowtarzania { get; set; } = "codziennie";
    public bool CzyAktywna { get; set; } = true;

    public ICollection<Zadanie> Zadania { get; set; } = new List<Zadanie>();
}
