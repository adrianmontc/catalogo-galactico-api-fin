using ApiCatalogo_Galactico.Data;
using ApiCatalogo_Galactico.Models;
using ApiCatalogo_Galactico.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo_Galactico.Services;

public class EstadisticaService : IEstadisticaService
{
    private readonly AppDbContext _db;

    public EstadisticaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<RankingPersonajeDto>> RankingPorPoderAsync(int limite)
    {
        return await _db.Cards
            .AsNoTracking()
            .Include(c => c.Personaje)
            .Where(c => c.Personaje != null)
            .OrderByDescending(c => c.Poder)
            .ThenByDescending(c => c.NivelPeligrosidad)
            .Take(limite)
            .Select(c => new RankingPersonajeDto
            {
                Id = c.Personaje!.Id,
                Nombre = c.Personaje.Nombre,
                Faccion = c.Personaje.Faccion,
                Poder = c.Poder,
                NivelPeligrosidad = c.NivelPeligrosidad
            })
            .ToListAsync();
    }

    public async Task<(MvpEventoDto? Mvp, bool EventoExiste)> ObtenerMvpAsync(int eventoId)
    {
        var evento = await _db.Eventos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventoId);

        if (evento is null)
            return (null, false);

        var mvp = await _db.Cards
            .AsNoTracking()
            .Include(c => c.Personaje)
            .Where(c => c.Personaje != null && evento.Participantes.Contains(c.PersonajeId))
            .OrderByDescending(c => c.Poder)
            .ThenByDescending(c => c.NivelPeligrosidad)
            .Select(c => new MvpEventoDto
            {
                EventoId = evento.Id,
                EventoNombre = evento.Nombre,
                PersonajeId = c.Personaje!.Id,
                PersonajeNombre = c.Personaje.Nombre,
                Faccion = c.Personaje.Faccion,
                Poder = c.Poder,
                HabilidadEspecial = c.HabilidadEspecial
            })
            .FirstOrDefaultAsync();

        return (mvp, true);
    }
}