using ApiCatalogo_Galactico.Dtos;

namespace ApiCatalogo_Galactico.Services
{
    public interface IEventoService
    {
        Task<IEnumerable<EventoDto>> ListarAsync(int? anioDesde, int? anioHasta);
        Task<EventoDto?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<PersonajeDto>?> ObtenerParticipantesAsync(int eventoId);
        Task<IEnumerable<EventoDto>> ObtenerPorPersonajeAsync(int personajeId);
        Task<(EventoDto? Creado, string? Error)> CrearAsync(EventoCrearDto dto);
        Task<(EventoDto? Actualizado, string? Error, bool Encontrado)> ActualizarAsync(int id, EventoCrearDto dto);
        Task<bool> EliminarAsync(int id);
    }
}
