namespace ApiCatalogo_Galactico.Dtos
{
    public class NaveDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int PoderFuego { get; set; }
        public string CoordenadasHangar { get; set; } = string.Empty;
        public int PersonajeId { get; set; }
        public string NombrePiloto { get; set; } = string.Empty;
    }

    public class NaveCrearDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int PoderFuego { get; set; }
        public string CodigoIgnicion { get; set; } = string.Empty;
        public string CoordenadasHangar { get; set; } = string.Empty;
        public int PersonajeId { get; set; }
    }

    public class NaveDesplegadaDto
    {
        public int NaveId { get; set; }
        public string NombreNave { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string NombrePiloto { get; set; } = string.Empty;
        public int PoderPiloto { get; set; }
        public int PoderFuegoNave { get; set; }
        public int PoderAereoCombinado { get; set; }
    }

    public class FaccionFlotaDto
    {
        public string Faccion { get; set; } = string.Empty;
        public int TotalPoderAereoFaccion { get; set; }
        public List<NaveDesplegadaDto> Naves { get; set; } = new();
    }

    public class DespliegueFlotaEventoDto
    {
        public int EventoId { get; set; }
        public string NombreEvento { get; set; } = string.Empty;
        public int Anio { get; set; }
        public List<FaccionFlotaDto> FlotaPorFaccion { get; set; } = new();
    }

    public class ExpedientePilotoDto
    {
        public int PersonajeId { get; set; }
        public string NombrePiloto { get; set; } = string.Empty;
        public string Faccion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string ArmaPersonal { get; set; } = string.Empty;
        public int NivelPeligrosidad { get; set; }
        public int PoderCarta { get; set; }
        public int PoderMejorNave { get; set; }
        public int EventosCombatidos { get; set; }
        public int IndiceEfectividad { get; set; }
        public List<NaveDto> FlotaAsignada { get; set; } = new();
    }

    public class RefuerzoDisponibleDto
    {
        public int PersonajeId { get; set; }
        public string NombrePiloto { get; set; } = string.Empty;
        public string Faccion { get; set; } = string.Empty;
        public string NombreMejorNave { get; set; } = string.Empty;
        public string ModeloNave { get; set; } = string.Empty;
        public int PoderCarta { get; set; }
        public int PoderFuegoNave { get; set; }
        public int PoderCombinado { get; set; }
    }
}
