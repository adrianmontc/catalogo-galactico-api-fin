namespace ApiCatalogo_Galactico.Models
{
    public class Nave
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int PoderFuego { get; set; }
        public string CodigoIgnicionHash { get; set; } = string.Empty;
        public string CoordenadasHangar { get; set; } = string.Empty;

        public int PersonajeId { get; set; }
        public Personaje? Personaje { get; set; }
    }
}
