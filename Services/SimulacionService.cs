using ApiCatalogo_Galactico.Data;
using ApiCatalogo_Galactico.Models;
using ApiCatalogo_Galactico.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo_Galactico.Services;

public class SimulacionService : ISimulacionService
{
    private readonly AppDbContext _db;
    private readonly Random _random = new();

    public SimulacionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(ResultadoSimulacionDto? Resultado, string? Error, bool EventoExiste)> SimularAsync(int eventoId)
    {
        var evento = await _db.Eventos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventoId);

        if (evento is null)
            return (null, null, false);

        var personajes = await _db.Personajes
            .AsNoTracking()
            .Include(p => p.Card)
            .Where(p => evento.Participantes.Contains(p.Id))
            .ToListAsync();

        if (personajes.Count != evento.Participantes.Count)
            return (null, "El evento contiene un personaje que ya no existe.", true);

        if (personajes.Any(p => p.Card is null))
            return (null, "No se puede simular: todos los participantes deben tener una carta.", true);

        var datos = personajes
            .Select(p => new ParticipanteSimulacionDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Faccion = p.Faccion,
                Poder = p.Card!.Poder
            })
            .ToList();

        var facciones = datos.Select(x => x.Faccion).Distinct().ToList();

        if (facciones.Count < 2)
            return (null, "La simulacion necesita participantes de al menos dos facciones diferentes.", true);

        var poderPorFaccion = datos
            .GroupBy(x => x.Faccion)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Poder));

        var factor = Math.Round(_random.NextDouble() * 0.20 + 0.90, 2);

        var poderAjustado = poderPorFaccion
            .ToDictionary(
                x => x.Key,
                x => Math.Round(x.Value * factor, 2));

        var ganador = poderAjustado
            .OrderByDescending(x => x.Value)
            .First()
            .Key;

        var criterio = $"Se sumo el poder de las cartas por faccion y se aplico un factor aleatorio acotado entre 0.90 y 1.10. La faccion con mayor poder ajustado gana: {ganador}.";

        var resultado = new ResultadoSimulacionDto
        {
            EventoId = evento.Id,
            EventoNombre = evento.Nombre,
            Participantes = datos,
            PoderPorFaccion = poderPorFaccion,
            PoderAjustado = poderAjustado,
            Ganador = ganador,
            Factor = factor,
            Criterio = criterio
        };

        return (resultado, null, true);
    }
}