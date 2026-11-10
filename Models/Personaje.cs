namespace ApiCatalogo_Galactico.Models;
public class Personaje
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string Faccion { get; set; } = string.Empty;
    public string Afiliacion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool FuerzaSensitivo { get; set; }
    public int? AnioMuerte { get; set; }

    public CardPersonaje? Card { get; set; }
    public ICollection<Nave> Naves { get; set; } = new List<Nave>();
}
