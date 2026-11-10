namespace ApiCatalogo_Galactico.Models;
public class CardPersonaje
{
    public int Id { get; set; }
    public int PersonajeId { get; set; }
    public int Poder { get; set; }
    public string HabilidadEspecial { get; set; } = string.Empty;
    public string Arma { get; set; } = string.Empty;
    public int NivelPeligrosidad { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
    public Personaje? Personaje { get; set; }
}