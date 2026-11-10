using ApiCatalogo_Galactico.Dtos;

namespace ApiCatalogo_Galactico.Services
{
    public interface ISimulacionService
    {
        Task<(ResultadoSimulacionDto? Resultado, string? Error, bool EventoExiste)> SimularAsync(int eventoId);
    }
}
