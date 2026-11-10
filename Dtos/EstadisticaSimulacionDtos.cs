namespace ApiCatalogo_Galactico.Dtos
{
    public class RankingPersonajeDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Faccion { get; set; } = string.Empty;
        public int Poder { get; set; }
        public int NivelPeligrosidad { get; set; }
    }

    public class MvpEventoDto
    {
        public int EventoId { get; set; }
        public string EventoNombre { get; set; } = string.Empty;
        public int PersonajeId { get; set; }
        public string PersonajeNombre { get; set; } = string.Empty;
        public string Faccion { get; set; } = string.Empty;
        public int Poder { get; set; }
        public string HabilidadEspecial { get; set; } = string.Empty;
    }

    public class ParticipanteSimulacionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Faccion { get; set; } = string.Empty;
        public int Poder { get; set; }
    }

    public class ResultadoSimulacionDto
    {
        public int EventoId { get; set; }
        public string EventoNombre { get; set; } = string.Empty;
        public List<ParticipanteSimulacionDto> Participantes { get; set; } = new();
        public Dictionary<string, int> PoderPorFaccion { get; set; } = new();
        public Dictionary<string, double> PoderAjustado { get; set; } = new();
        public string Ganador { get; set; } = string.Empty;
        public double Factor { get; set; }
        public string Criterio { get; set; } = string.Empty;
    }
}
