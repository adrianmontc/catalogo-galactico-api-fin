namespace ApiCatalogo_Galactico.Dtos
{
    public class PersonajeDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty;
        public string Faccion { get; set; } = string.Empty;
        public string Afiliacion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool FuerzaSensitivo { get; set; }
        public int? AnioMuerte { get; set; }
    }

    public class PersonajeCrearDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty;
        public string Faccion { get; set; } = string.Empty;
        public string Afiliacion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool FuerzaSensitivo { get; set; }
        public int? AnioMuerte { get; set; }
    }
}
