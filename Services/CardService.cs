using ApiCatalogo_Galactico.Data;
using ApiCatalogo_Galactico.Models;
using ApiCatalogo_Galactico.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo_Galactico.Services;

public class CardService : ICardService
{
    private readonly AppDbContext _db;

    public CardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CardDto>> ListarAsync()
    {
        return await _db.Cards
            .AsNoTracking()
            .OrderByDescending(c => c.Poder)
            .Select(c => new CardDto
            {
                Id = c.Id,
                PersonajeId = c.PersonajeId,
                Poder = c.Poder,
                HabilidadEspecial = c.HabilidadEspecial,
                Arma = c.Arma,
                NivelPeligrosidad = c.NivelPeligrosidad,
                ImagenUrl = c.ImagenUrl
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<CardDto>> MasPoderosasAsync(int limite)
    {
        return await _db.Cards
            .AsNoTracking()
            .OrderByDescending(c => c.Poder)
            .ThenByDescending(c => c.NivelPeligrosidad)
            .Take(limite)
            .Select(c => new CardDto
            {
                Id = c.Id,
                PersonajeId = c.PersonajeId,
                Poder = c.Poder,
                HabilidadEspecial = c.HabilidadEspecial,
                Arma = c.Arma,
                NivelPeligrosidad = c.NivelPeligrosidad,
                ImagenUrl = c.ImagenUrl
            })
            .ToListAsync();
    }

    public async Task<CardDto?> ObtenerPorIdAsync(int id)
    {
        return await _db.Cards
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CardDto
            {
                Id = c.Id,
                PersonajeId = c.PersonajeId,
                Poder = c.Poder,
                HabilidadEspecial = c.HabilidadEspecial,
                Arma = c.Arma,
                NivelPeligrosidad = c.NivelPeligrosidad,
                ImagenUrl = c.ImagenUrl
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CardDto?> ObtenerPorPersonajeAsync(int personajeId)
    {
        return await _db.Cards
            .AsNoTracking()
            .Where(c => c.PersonajeId == personajeId)
            .Select(c => new CardDto
            {
                Id = c.Id,
                PersonajeId = c.PersonajeId,
                Poder = c.Poder,
                HabilidadEspecial = c.HabilidadEspecial,
                Arma = c.Arma,
                NivelPeligrosidad = c.NivelPeligrosidad,
                ImagenUrl = c.ImagenUrl
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(CardDto? Creada, string? Error, bool Conflicto)> CrearAsync(CardCrearDto dto)
    {
        var error = ValidacionService.ValidarCard(
            dto.PersonajeId,
            dto.Poder,
            dto.HabilidadEspecial,
            dto.Arma,
            dto.NivelPeligrosidad,
            dto.ImagenUrl);

        if (error is not null)
            return (null, error, false);

        var existePersonaje = await _db.Personajes.AnyAsync(p => p.Id == dto.PersonajeId);
        if (!existePersonaje)
            return (null, "El personaje indicado no existe.", false);

        var yaTieneCarta = await _db.Cards.AnyAsync(c => c.PersonajeId == dto.PersonajeId);
        if (yaTieneCarta)
            return (null, "El personaje ya tiene una carta asociada.", true);

        var card = new CardPersonaje
        {
            PersonajeId = dto.PersonajeId,
            Poder = dto.Poder,
            HabilidadEspecial = dto.HabilidadEspecial.Trim(),
            Arma = dto.Arma.Trim(),
            NivelPeligrosidad = dto.NivelPeligrosidad,
            ImagenUrl = dto.ImagenUrl.Trim()
        };

        _db.Cards.Add(card);
        await _db.SaveChangesAsync();

        var resultado = new CardDto
        {
            Id = card.Id,
            PersonajeId = card.PersonajeId,
            Poder = card.Poder,
            HabilidadEspecial = card.HabilidadEspecial,
            Arma = card.Arma,
            NivelPeligrosidad = card.NivelPeligrosidad,
            ImagenUrl = card.ImagenUrl
        };

        return (resultado, null, false);
    }

    public async Task<(CardDto? Actualizada, string? Error, bool Encontrada, bool Conflicto)> ActualizarAsync(int id, CardCrearDto dto)
    {
        var card = await _db.Cards.FindAsync(id);
        if (card is null)
            return (null, null, false, false);

        var error = ValidacionService.ValidarCard(
            dto.PersonajeId,
            dto.Poder,
            dto.HabilidadEspecial,
            dto.Arma,
            dto.NivelPeligrosidad,
            dto.ImagenUrl);

        if (error is not null)
            return (null, error, true, false);

        var existePersonaje = await _db.Personajes.AnyAsync(p => p.Id == dto.PersonajeId);
        if (!existePersonaje)
            return (null, "El personaje indicado no existe.", true, false);

        var otraCarta = await _db.Cards.AnyAsync(c => c.PersonajeId == dto.PersonajeId && c.Id != id);
        if (otraCarta)
            return (null, "El personaje ya tiene otra carta asociada.", true, true);

        card.PersonajeId = dto.PersonajeId;
        card.Poder = dto.Poder;
        card.HabilidadEspecial = dto.HabilidadEspecial.Trim();
        card.Arma = dto.Arma.Trim();
        card.NivelPeligrosidad = dto.NivelPeligrosidad;
        card.ImagenUrl = dto.ImagenUrl.Trim();

        await _db.SaveChangesAsync();

        var resultado = new CardDto
        {
            Id = card.Id,
            PersonajeId = card.PersonajeId,
            Poder = card.Poder,
            HabilidadEspecial = card.HabilidadEspecial,
            Arma = card.Arma,
            NivelPeligrosidad = card.NivelPeligrosidad,
            ImagenUrl = card.ImagenUrl
        };

        return (resultado, null, true, false);
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var card = await _db.Cards.FindAsync(id);
        if (card is null)
            return false;

        _db.Cards.Remove(card);
        await _db.SaveChangesAsync();
        return true;
    }
}
