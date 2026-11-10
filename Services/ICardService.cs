using ApiCatalogo_Galactico.Dtos;

namespace ApiCatalogo_Galactico.Services
{
    public interface ICardService
    {
        Task<IEnumerable<CardDto>> ListarAsync();
        Task<IEnumerable<CardDto>> MasPoderosasAsync(int limite);
        Task<CardDto?> ObtenerPorIdAsync(int id);
        Task<CardDto?> ObtenerPorPersonajeAsync(int personajeId);
        Task<(CardDto? Creada, string? Error, bool Conflicto)> CrearAsync(CardCrearDto dto);
        Task<(CardDto? Actualizada, string? Error, bool Encontrada, bool Conflicto)> ActualizarAsync(int id, CardCrearDto dto);
        Task<bool> EliminarAsync(int id);
    }
}