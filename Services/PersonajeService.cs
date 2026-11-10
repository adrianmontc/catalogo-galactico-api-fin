using ApiCatalogo_Galactico.Data;
using ApiCatalogo_Galactico.Models;
using ApiCatalogo_Galactico.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo_Galactico.Services;

public class PersonajeService : IPersonajeService
{
    private readonly AppDbContext _db;

    public PersonajeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<PersonajeDto>> ListarAsync(string? faccion, bool? fuerzaSensitivo, string? estado)
    {
        var query = _db.Personajes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(faccion))
            query = query.Where(p => p.Faccion == faccion);

        if (fuerzaSensitivo.HasValue)
            query = query.Where(p => p.FuerzaSensitivo == fuerzaSensitivo.Value);

        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(p => p.Estado == estado);

        return await query
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

    public async Task<PersonajeDto?> ObtenerPorIdAsync(int id)
    {
        return await _db.Personajes
            .AsNoTracking()
            .Where(p => p.Id == id)
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
            .FirstOrDefaultAsync();
    }

    public async Task<(PersonajeDto? Creado, string? Error)> CrearAsync(PersonajeCrearDto dto)
    {
        var error = ValidacionService.ValidarPersonaje(
            dto.Nombre,
            dto.Especie,
            dto.Faccion,
            dto.Afiliacion,
            dto.Estado,
            dto.AnioMuerte);

        if (error is not null)
            return (null, error);

        var personaje = new Personaje
        {
            Nombre = dto.Nombre.Trim(),
            Especie = dto.Especie.Trim(),
            Faccion = dto.Faccion,
            Afiliacion = dto.Afiliacion.Trim(),
            Estado = dto.Estado,
            FuerzaSensitivo = dto.FuerzaSensitivo,
            AnioMuerte = dto.AnioMuerte
        };

        _db.Personajes.Add(personaje);
        await _db.SaveChangesAsync();

        var resultado = new PersonajeDto
        {
            Id = personaje.Id,
            Nombre = personaje.Nombre,
            Especie = personaje.Especie,
            Faccion = personaje.Faccion,
            Afiliacion = personaje.Afiliacion,
            Estado = personaje.Estado,
            FuerzaSensitivo = personaje.FuerzaSensitivo,
            AnioMuerte = personaje.AnioMuerte
        };

        return (resultado, null);
    }

    public async Task<(PersonajeDto? Actualizado, string? Error, bool Encontrado)> ActualizarAsync(int id, PersonajeCrearDto dto)
    {
        var personaje = await _db.Personajes.FindAsync(id);
        if (personaje is null)
            return (null, null, false);

        var error = ValidacionService.ValidarPersonaje(
            dto.Nombre,
            dto.Especie,
            dto.Faccion,
            dto.Afiliacion,
            dto.Estado,
            dto.AnioMuerte);

        if (error is not null)
            return (null, error, true);

        personaje.Nombre = dto.Nombre.Trim();
        personaje.Especie = dto.Especie.Trim();
        personaje.Faccion = dto.Faccion;
        personaje.Afiliacion = dto.Afiliacion.Trim();
        personaje.Estado = dto.Estado;
        personaje.FuerzaSensitivo = dto.FuerzaSensitivo;
        personaje.AnioMuerte = dto.AnioMuerte;

        await _db.SaveChangesAsync();

        var resultado = new PersonajeDto
        {
            Id = personaje.Id,
            Nombre = personaje.Nombre,
            Especie = personaje.Especie,
            Faccion = personaje.Faccion,
            Afiliacion = personaje.Afiliacion,
            Estado = personaje.Estado,
            FuerzaSensitivo = personaje.FuerzaSensitivo,
            AnioMuerte = personaje.AnioMuerte
        };

        return (resultado, null, true);
    }

    public async Task<ResultadoEliminacionPersonaje> EliminarAsync(int id)
    {
        var personaje = await _db.Personajes.FindAsync(id);
        if (personaje is null)
            return ResultadoEliminacionPersonaje.NoEncontrado;

        var tieneCarta = await _db.Cards.AnyAsync(c => c.PersonajeId == id);
        if (tieneCarta)
            return ResultadoEliminacionPersonaje.TieneCarta;

        var eventos = await _db.Eventos.AsNoTracking().ToListAsync();
        if (eventos.Any(e => e.Participantes.Contains(id)))
            return ResultadoEliminacionPersonaje.ParticipaEnEventos;

        _db.Personajes.Remove(personaje);
        await _db.SaveChangesAsync();
        return ResultadoEliminacionPersonaje.Eliminado;
    }
}