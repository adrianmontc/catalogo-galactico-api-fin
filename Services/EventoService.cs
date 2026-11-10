using ApiCatalogo_Galactico.Data;
using ApiCatalogo_Galactico.Models;
using ApiCatalogo_Galactico.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo_Galactico.Services;

public class EventoService : IEventoService
{
    private readonly AppDbContext _db;

    public EventoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<EventoDto>> ListarAsync(int? anioDesde, int? anioHasta)
    {
        var query = _db.Eventos.AsNoTracking().AsQueryable();

        if (anioDesde.HasValue)
            query = query.Where(e => e.Anio >= anioDesde.Value);

        if (anioHasta.HasValue)
            query = query.Where(e => e.Anio <= anioHasta.Value);

        return await query
            .OrderBy(e => e.Anio)
            .ThenBy(e => e.Id)
            .Select(e => new EventoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Anio = e.Anio,
                Ubicacion = e.Ubicacion,
                Descripcion = e.Descripcion,
                Participantes = e.Participantes,
                Muertos = e.Muertos,
                Resultado = e.Resultado,
                Ganador = e.Ganador
            })
            .ToListAsync();
    }

    public async Task<EventoDto?> ObtenerPorIdAsync(int id)
    {
        return await _db.Eventos
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EventoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Anio = e.Anio,
                Ubicacion = e.Ubicacion,
                Descripcion = e.Descripcion,
                Participantes = e.Participantes,
                Muertos = e.Muertos,
                Resultado = e.Resultado,
                Ganador = e.Ganador
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<PersonajeDto>?> ObtenerParticipantesAsync(int eventoId)
    {
        var evento = await _db.Eventos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == eventoId);
        if (evento is null)
            return null;

        return await _db.Personajes
            .AsNoTracking()
            .Where(p => evento.Participantes.Contains(p.Id))
            .OrderBy(p => p.Id)
            .Select(p => new PersonajeDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Especie = p.Especie,
                Faccion = p.Faccion,
                Afiliacion = p.Afiliacion,
                Estado = p.Estado,
                FuerzaSensitivo = p.FuerzaSensitivo,
                AnioMuerte = p.AnioMuerte
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<EventoDto>> ObtenerPorPersonajeAsync(int personajeId)
    {
        var eventos = await _db.Eventos
            .AsNoTracking()
            .OrderBy(e => e.Anio)
            .ToListAsync();

        return eventos
            .Where(e => e.Participantes.Contains(personajeId))
            .Select(e => new EventoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Anio = e.Anio,
                Ubicacion = e.Ubicacion,
                Descripcion = e.Descripcion,
                Participantes = e.Participantes,
                Muertos = e.Muertos,
                Resultado = e.Resultado,
                Ganador = e.Ganador
            });
    }

    public async Task<(EventoDto? Creado, string? Error)> CrearAsync(EventoCrearDto dto)
    {
        var error = ValidacionService.ValidarEvento(dto.Nombre, dto.Ubicacion, dto.Descripcion, dto.Anio);
        if (error is not null)
            return (null, error);

        var errorParticipantes = await ValidarParticipantesAsync(dto.Participantes, dto.Anio, dto.Muertos);
        if (errorParticipantes is not null)
            return (null, errorParticipantes);

        var evento = new Evento
        {
            Nombre = dto.Nombre.Trim(),
            Anio = dto.Anio,
            Ubicacion = dto.Ubicacion.Trim(),
            Descripcion = dto.Descripcion.Trim(),
            Participantes = dto.Participantes.Distinct().ToList(),
            Muertos = dto.Muertos.Distinct().ToList(),
            Resultado = string.IsNullOrWhiteSpace(dto.Resultado) ? null : dto.Resultado.Trim(),
            Ganador = string.IsNullOrWhiteSpace(dto.Ganador) ? null : dto.Ganador.Trim()
        };

        _db.Eventos.Add(evento);

        var personajesMuertos = await _db.Personajes
            .Where(p => evento.Muertos.Contains(p.Id))
            .ToListAsync();

        foreach (var personaje in personajesMuertos)
        {
            personaje.Estado = "muerto";
            personaje.AnioMuerte = evento.Anio;
        }

        await _db.SaveChangesAsync();

        var resultado = new EventoDto
        {
            Id = evento.Id,
            Nombre = evento.Nombre,
            Anio = evento.Anio,
            Ubicacion = evento.Ubicacion,
            Descripcion = evento.Descripcion,
            Participantes = evento.Participantes,
            Muertos = evento.Muertos,
            Resultado = evento.Resultado,
            Ganador = evento.Ganador
        };

        return (resultado, null);
    }

    public async Task<(EventoDto? Actualizado, string? Error, bool Encontrado)> ActualizarAsync(int id, EventoCrearDto dto)
    {
        var evento = await _db.Eventos.FindAsync(id);
        if (evento is null)
            return (null, null, false);

        var error = ValidacionService.ValidarEvento(dto.Nombre, dto.Ubicacion, dto.Descripcion, dto.Anio);
        if (error is not null)
            return (null, error, true);

        var errorParticipantes = await ValidarParticipantesAsync(dto.Participantes, dto.Anio, dto.Muertos);
        if (errorParticipantes is not null)
            return (null, errorParticipantes, true);

        evento.Nombre = dto.Nombre.Trim();
        evento.Anio = dto.Anio;
        evento.Ubicacion = dto.Ubicacion.Trim();
        evento.Descripcion = dto.Descripcion.Trim();
        evento.Participantes = dto.Participantes.Distinct().ToList();
        evento.Muertos = dto.Muertos.Distinct().ToList();
        evento.Resultado = string.IsNullOrWhiteSpace(dto.Resultado) ? null : dto.Resultado.Trim();
        evento.Ganador = string.IsNullOrWhiteSpace(dto.Ganador) ? null : dto.Ganador.Trim();

        var personajesMuertos = await _db.Personajes
            .Where(p => evento.Muertos.Contains(p.Id))
            .ToListAsync();

        foreach (var personaje in personajesMuertos)
        {
            personaje.Estado = "muerto";
            personaje.AnioMuerte = evento.Anio;
        }

        await _db.SaveChangesAsync();

        var resultado = new EventoDto
        {
            Id = evento.Id,
            Nombre = evento.Nombre,
            Anio = evento.Anio,
            Ubicacion = evento.Ubicacion,
            Descripcion = evento.Descripcion,
            Participantes = evento.Participantes,
            Muertos = evento.Muertos,
            Resultado = evento.Resultado,
            Ganador = evento.Ganador
        };

        return (resultado, null, true);
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var evento = await _db.Eventos.FindAsync(id);
        if (evento is null)
            return false;

        _db.Eventos.Remove(evento);
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task<string?> ValidarParticipantesAsync(List<int> participantes, int anio, List<int> muertos)
    {
        if (participantes.Count < 2)
            return "Un evento debe tener al menos 2 participantes.";

        if (participantes.Distinct().Count() != participantes.Count)
            return "No se permiten participantes repetidos.";

        if (muertos.Distinct().Count() != muertos.Count)
            return "No se permiten personajes muertos repetidos.";

        if (muertos.Any(id => !participantes.Contains(id)))
            return "Todo personaje incluido en Muertos debe ser participante del evento.";

        var personajesDb = await _db.Personajes
            .AsNoTracking()
            .Where(p => participantes.Contains(p.Id))
            .ToListAsync();

        if (personajesDb.Count != participantes.Count)
            return "Uno o más personajes participantes no existen.";

        foreach (var personaje in personajesDb)
        {
            if (personaje.Estado == "muerto" &&
                personaje.AnioMuerte.HasValue &&
                anio > personaje.AnioMuerte.Value)
            {
                return $"El personaje {personaje.Nombre} no puede participar en un evento posterior a su muerte en {personaje.AnioMuerte.Value}.";
            }
        }

        return null;
    }
}