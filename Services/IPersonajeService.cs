using ApiCatalogo_Galactico.Dtos;

namespace ApiCatalogo_Galactico.Services;

public enum ResultadoEliminacionPersonaje
{
    Eliminado,
    NoEncontrado,
    TieneCarta,
    ParticipaEnEventos,
    TieneNaves
}

public interface IPersonajeService
{
    Task<IEnumerable<PersonajeDto>> ListarAsync(string? faccion, bool? fuerzaSensitivo, string? estado);
    Task<PersonajeDto?> ObtenerPorIdAsync(int id);
    Task<(PersonajeDto? Creado, string? Error)> CrearAsync(PersonajeCrearDto dto);
    Task<(PersonajeDto? Actualizado, string? Error, bool Encontrado)> ActualizarAsync(int id, PersonajeCrearDto dto);
    Task<ResultadoEliminacionPersonaje> EliminarAsync(int id);
}
