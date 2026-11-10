using ApiCatalogo_Galactico.Dtos;
using ApiCatalogo_Galactico.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo_Galactico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonajesController : ControllerBase
    {
        private readonly IPersonajeService _personajeService;
        private readonly ICardService _cardService;
        private readonly IEventoService _eventoService;
        private readonly IEstadisticaService _estadisticaService;
        private readonly INaveService _naveService;

        public PersonajesController(
            IPersonajeService personajeService,
            ICardService cardService,
            IEventoService eventoService,
            IEstadisticaService estadisticaService,
            INaveService naveService)
        {
            _personajeService = personajeService;
            _cardService = cardService;
            _eventoService = eventoService;
            _estadisticaService = estadisticaService;
            _naveService = naveService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonajeDto>>> Listar(
            [FromQuery] string? faccion,
            [FromQuery] bool? fuerzaSensitivo,
            [FromQuery] string? estado)
        {
            if (!string.IsNullOrWhiteSpace(faccion) && !ValidacionService.Facciones.Contains(faccion))
                return BadRequest(new { mensaje = "La facción debe ser Rebelde, Imperio o Neutral." });

            if (!string.IsNullOrWhiteSpace(estado) && !ValidacionService.Estados.Contains(estado))
                return BadRequest(new { mensaje = "El estado debe ser vivo, muerto o desconocido." });

            return Ok(await _personajeService.ListarAsync(faccion, fuerzaSensitivo, estado));
        }

        [HttpGet("ranking")]
        public async Task<ActionResult<IEnumerable<RankingPersonajeDto>>> Ranking(
            [FromQuery] string? por,
            [FromQuery] int? limite)
        {
            if (!string.Equals(por, "poder", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { mensaje = "El parámetro 'por' debe ser 'poder'." });

            var cantidad = limite ?? 10;

            if (cantidad < 1 || cantidad > 50)
                return BadRequest(new { mensaje = "El límite debe estar entre 1 y 50." });

            return Ok(await _estadisticaService.RankingPorPoderAsync(cantidad));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PersonajeDto>> ObtenerPorId(int id)
        {
            var personaje = await _personajeService.ObtenerPorIdAsync(id);
            return personaje is null ? NotFound() : Ok(personaje);
        }

        [HttpGet("{id:int}/carta")]
        public async Task<ActionResult<CardDto>> ObtenerCarta(int id)
        {
            var personaje = await _personajeService.ObtenerPorIdAsync(id);
            if (personaje is null)
                return NotFound(new { mensaje = "Personaje no encontrado." });

            var card = await _cardService.ObtenerPorPersonajeAsync(id);
            return card is null ? NotFound(new { mensaje = "El personaje no tiene una carta asociada." }) : Ok(card);
        }

        [HttpGet("{id:int}/eventos")]
        public async Task<ActionResult<IEnumerable<EventoDto>>> ObtenerEventos(int id)
        {
            var personaje = await _personajeService.ObtenerPorIdAsync(id);
            if (personaje is null)
                return NotFound(new { mensaje = "Personaje no encontrado." });

            return Ok(await _eventoService.ObtenerPorPersonajeAsync(id));
        }

        [HttpGet("{id:int}/naves")]
        public async Task<ActionResult<IEnumerable<NaveDto>>> ObtenerNaves(int id)
        {
            var personaje = await _personajeService.ObtenerPorIdAsync(id);
            if (personaje is null)
                return NotFound(new { mensaje = "Personaje no encontrado." });

            return Ok(await _naveService.ObtenerPorPersonajeAsync(id));
        }

        [HttpGet("{id:int}/expediente-piloto")]
        public async Task<ActionResult<ExpedientePilotoDto>> ObtenerExpedientePiloto(int id)
        {
            var expediente = await _naveService.ObtenerExpedientePilotoAsync(id);
            return expediente is null ? NotFound(new { mensaje = "Personaje no encontrado." }) : Ok(expediente);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PersonajeDto>> Crear(PersonajeCrearDto dto)
        {
            var (creado, error) = await _personajeService.CrearAsync(dto);
            if (error is not null)
                return BadRequest(new { mensaje = error });

            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado!.Id }, creado);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PersonajeDto>> Actualizar(int id, PersonajeCrearDto dto)
        {
            var (actualizado, error, encontrado) = await _personajeService.ActualizarAsync(id, dto);
            if (!encontrado)
                return NotFound();

            if (error is not null)
                return BadRequest(new { mensaje = error });

            return Ok(actualizado);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _personajeService.EliminarAsync(id);
            return resultado switch
            {
                ResultadoEliminacionPersonaje.Eliminado => NoContent(),
                ResultadoEliminacionPersonaje.TieneCarta => Conflict(new { mensaje = "No se puede eliminar el personaje porque tiene una carta asociada." }),
                ResultadoEliminacionPersonaje.TieneNaves => Conflict(new { mensaje = "No se puede eliminar el personaje porque tiene naves registradas a su mando." }),
                ResultadoEliminacionPersonaje.ParticipaEnEventos => Conflict(new { mensaje = "No se puede eliminar el personaje porque participa en uno o más eventos." }),
                _ => NotFound()
            };
        }
    }
}

