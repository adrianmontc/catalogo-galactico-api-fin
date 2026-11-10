using ApiCatalogo_Galactico.Dtos;

namespace ApiCatalogo_Galactico.Services
{
    public interface INaveService
    {
        Task<IEnumerable<NaveDto>> ListarAsync();
        Task<NaveDto?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<NaveDto>> ObtenerPorPersonajeAsync(int personajeId);
        Task<(NaveDto? Creada, string? Error)> CrearAsync(NaveCrearDto dto);
        Task<(NaveDto? Actualizada, string? Error, bool Encontrada)> ActualizarAsync(int id, NaveCrearDto dto);
        Task<bool> EliminarAsync(int id);
        Task<DespliegueFlotaEventoDto?> ObtenerDespliegueFlotaAsync(int eventoId);
        Task<ExpedientePilotoDto?> ObtenerExpedientePilotoAsync(int personajeId);
        Task<IEnumerable<RefuerzoDisponibleDto>?> ObtenerRefuerzosDisponiblesAsync(int eventoId, int poderMinimo);
    }
}
