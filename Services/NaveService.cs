using ApiCatalogo_Galactico.Data;
using ApiCatalogo_Galactico.Dtos;
using ApiCatalogo_Galactico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo_Galactico.Services
{
    public class NaveService : INaveService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<Nave> _passwordHasher;
        private readonly ICifradoService _cifrado;

        public NaveService(
            AppDbContext db,
            IPasswordHasher<Nave> passwordHasher,
            ICifradoService cifrado)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _cifrado = cifrado;
        }

        public async Task<IEnumerable<NaveDto>> ListarAsync()
        {
            var naves = await _db.Naves
                .AsNoTracking()
                .Include(n => n.Personaje)
                .OrderByDescending(n => n.PoderFuego)
                .ToListAsync();

            return naves.Select(MapearDto);
        }

        public async Task<NaveDto?> ObtenerPorIdAsync(int id)
        {
            var nave = await _db.Naves
                .AsNoTracking()
                .Include(n => n.Personaje)
                .FirstOrDefaultAsync(n => n.Id == id);

            return nave is null ? null : MapearDto(nave);
        }

        public async Task<IEnumerable<NaveDto>> ObtenerPorPersonajeAsync(int personajeId)
        {
            var naves = await _db.Naves
                .AsNoTracking()
                .Include(n => n.Personaje)
                .Where(n => n.PersonajeId == personajeId)
                .OrderByDescending(n => n.PoderFuego)
                .ToListAsync();

            return naves.Select(MapearDto);
        }

        public async Task<(NaveDto? Creada, string? Error)> CrearAsync(NaveCrearDto dto)
        {
            var error = ValidarNave(dto);
            if (error is not null)
                return (null, error);

            var piloto = await _db.Personajes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == dto.PersonajeId);
            if (piloto is null)
                return (null, "El personaje piloto indicado no existe.");

            var nave = new Nave
            {
                Nombre = dto.Nombre.Trim(),
                Modelo = dto.Modelo.Trim(),
                PoderFuego = dto.PoderFuego,
                CoordenadasHangar = _cifrado.Cifrar(dto.CoordenadasHangar.Trim()),
                PersonajeId = dto.PersonajeId
            };

            nave.CodigoIgnicionHash = _passwordHasher.HashPassword(nave, dto.CodigoIgnicion);

            _db.Naves.Add(nave);
            await _db.SaveChangesAsync();

            nave.Personaje = piloto;
            return (MapearDto(nave), null);
        }

        public async Task<(NaveDto? Actualizada, string? Error, bool Encontrada)> ActualizarAsync(int id, NaveCrearDto dto)
        {
            var nave = await _db.Naves.FindAsync(id);
            if (nave is null)
                return (null, null, false);

            var error = ValidarNave(dto);
            if (error is not null)
                return (null, error, true);

            var piloto = await _db.Personajes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == dto.PersonajeId);
            if (piloto is null)
                return (null, "El personaje piloto indicado no existe.", true);

            nave.Nombre = dto.Nombre.Trim();
            nave.Modelo = dto.Modelo.Trim();
            nave.PoderFuego = dto.PoderFuego;
            nave.CoordenadasHangar = _cifrado.Cifrar(dto.CoordenadasHangar.Trim());
            nave.PersonajeId = dto.PersonajeId;
            nave.CodigoIgnicionHash = _passwordHasher.HashPassword(nave, dto.CodigoIgnicion);

            await _db.SaveChangesAsync();

            nave.Personaje = piloto;
            return (MapearDto(nave), null, true);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var nave = await _db.Naves.FindAsync(id);
            if (nave is null)
                return false;

            _db.Naves.Remove(nave);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<DespliegueFlotaEventoDto?> ObtenerDespliegueFlotaAsync(int eventoId)
        {
            var evento = await _db.Eventos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == eventoId);
            if (evento is null)
                return null;

            var personajes = await _db.Personajes
                .AsNoTracking()
                .Include(p => p.Card)
                .Include(p => p.Naves)
                .Where(p => evento.Participantes.Contains(p.Id))
                .ToListAsync();

            var flotaPorFaccion = personajes
                .SelectMany(p => p.Naves.Select(n => new
                {
                    Faccion = p.Faccion,
                    Detalle = new NaveDesplegadaDto
                    {
                        NaveId = n.Id,
                        NombreNave = n.Nombre,
                        Modelo = n.Modelo,
                        NombrePiloto = p.Nombre,
                        PoderPiloto = p.Card?.Poder ?? 0,
                        PoderFuegoNave = n.PoderFuego,
                        PoderAereoCombinado = (p.Card?.Poder ?? 0) + n.PoderFuego
                    }
                }))
                .GroupBy(x => x.Faccion)
                .Select(g => new FaccionFlotaDto
                {
                    Faccion = g.Key,
                    TotalPoderAereoFaccion = g.Sum(x => x.Detalle.PoderAereoCombinado),
                    Naves = g.Select(x => x.Detalle).OrderByDescending(d => d.PoderAereoCombinado).ToList()
                })
                .OrderByDescending(f => f.TotalPoderAereoFaccion)
                .ToList();

            return new DespliegueFlotaEventoDto
            {
                EventoId = evento.Id,
                NombreEvento = evento.Nombre,
                Anio = evento.Anio,
                FlotaPorFaccion = flotaPorFaccion
            };
        }

        public async Task<ExpedientePilotoDto?> ObtenerExpedientePilotoAsync(int personajeId)
        {
            var personaje = await _db.Personajes
                .AsNoTracking()
                .Include(p => p.Card)
                .Include(p => p.Naves)
                .FirstOrDefaultAsync(p => p.Id == personajeId);

            if (personaje is null)
                return null;

            var todosEventos = await _db.Eventos.AsNoTracking().ToListAsync();
            var eventosCombatidos = todosEventos.Count(e => e.Participantes.Contains(personajeId));

            var poderCarta = personaje.Card?.Poder ?? 0;
            var poderMejorNave = personaje.Naves.Any() ? personaje.Naves.Max(n => n.PoderFuego) : 0;
            var bonoExperiencia = eventosCombatidos * 10;

            var flotaDescifrada = personaje.Naves
                .OrderByDescending(n => n.PoderFuego)
                .Select(n => new NaveDto
                {
                    Id = n.Id,
                    Nombre = n.Nombre,
                    Modelo = n.Modelo,
                    PoderFuego = n.PoderFuego,
                    CoordenadasHangar = _cifrado.Descifrar(n.CoordenadasHangar),
                    PersonajeId = personaje.Id,
                    NombrePiloto = personaje.Nombre
                })
                .ToList();

            return new ExpedientePilotoDto
            {
                PersonajeId = personaje.Id,
                NombrePiloto = personaje.Nombre,
                Faccion = personaje.Faccion,
                Estado = personaje.Estado,
                ArmaPersonal = personaje.Card?.Arma ?? "Sin carta registrada",
                NivelPeligrosidad = personaje.Card?.NivelPeligrosidad ?? 0,
                PoderCarta = poderCarta,
                PoderMejorNave = poderMejorNave,
                EventosCombatidos = eventosCombatidos,
                IndiceEfectividad = poderCarta + poderMejorNave + bonoExperiencia,
                FlotaAsignada = flotaDescifrada
            };
        }

        public async Task<IEnumerable<RefuerzoDisponibleDto>?> ObtenerRefuerzosDisponiblesAsync(int eventoId, int poderMinimo)
        {
            var evento = await _db.Eventos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == eventoId);
            if (evento is null)
                return null;

            var candidatos = await _db.Personajes
                .AsNoTracking()
                .Include(p => p.Card)
                .Include(p => p.Naves)
                .Where(p => !evento.Participantes.Contains(p.Id))
                .Where(p => p.Card != null && p.Naves.Any())
                .ToListAsync();

            return candidatos
                .Where(p => !(p.Estado == "muerto" && p.AnioMuerte.HasValue && p.AnioMuerte.Value < evento.Anio))
                .Select(p =>
                {
                    var mejorNave = p.Naves.OrderByDescending(n => n.PoderFuego).First();
                    var poderCarta = p.Card!.Poder;
                    return new RefuerzoDisponibleDto
                    {
                        PersonajeId = p.Id,
                        NombrePiloto = p.Nombre,
                        Faccion = p.Faccion,
                        NombreMejorNave = mejorNave.Nombre,
                        ModeloNave = mejorNave.Modelo,
                        PoderCarta = poderCarta,
                        PoderFuegoNave = mejorNave.PoderFuego,
                        PoderCombinado = poderCarta + mejorNave.PoderFuego
                    };
                })
                .Where(r => r.PoderCombinado >= poderMinimo)
                .OrderByDescending(r => r.PoderCombinado)
                .ToList();
        }

        private NaveDto MapearDto(Nave nave)
        {
            return new NaveDto
            {
                Id = nave.Id,
                Nombre = nave.Nombre,
                Modelo = nave.Modelo,
                PoderFuego = nave.PoderFuego,
                CoordenadasHangar = _cifrado.Descifrar(nave.CoordenadasHangar),
                PersonajeId = nave.PersonajeId,
                NombrePiloto = nave.Personaje?.Nombre ?? string.Empty
            };
        }

        private static string? ValidarNave(NaveCrearDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return "El nombre de la nave es obligatorio.";

            if (string.IsNullOrWhiteSpace(dto.Modelo))
                return "El modelo de la nave es obligatorio.";

            if (dto.PoderFuego < 1 || dto.PoderFuego > 100)
                return "El poder de fuego debe estar entre 1 y 100.";

            if (string.IsNullOrWhiteSpace(dto.CodigoIgnicion) || dto.CodigoIgnicion.Length < 4)
                return "El código de ignición debe tener al menos 4 caracteres.";

            if (string.IsNullOrWhiteSpace(dto.CoordenadasHangar))
                return "Las coordenadas del hangar son obligatorias.";

            if (dto.PersonajeId <= 0)
                return "El PersonajeId no es válido.";

            return null;
        }
    }
}
