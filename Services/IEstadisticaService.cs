using ApiCatalogo_Galactico.Dtos;

namespace ApiCatalogo_Galactico.Services
{
    public interface IEstadisticaService
    {
        Task<IEnumerable<RankingPersonajeDto>> RankingPorPoderAsync(int limite);
        Task<(MvpEventoDto? Mvp, bool EventoExiste)> ObtenerMvpAsync(int eventoId);
    }
}
