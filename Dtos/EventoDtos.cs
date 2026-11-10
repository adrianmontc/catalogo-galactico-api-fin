namespace ApiCatalogo_Galactico.Dtos
{
    public class EventoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Ubicacion { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public List<int> Participantes { get; set; } = new();
        public List<int> Muertos { get; set; } = new();
        public string? Resultado { get; set; }
        public string? Ganador { get; set; }
    }

    public class EventoCrearDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Ubicacion { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public List<int> Participantes { get; set; } = new();
        public List<int> Muertos { get; set; } = new();
        public string? Resultado { get; set; }
        public string? Ganador { get; set; }
    }
}
