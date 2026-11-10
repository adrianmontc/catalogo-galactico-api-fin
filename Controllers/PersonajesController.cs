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

        public PersonajesController(
            IPersonajeService personajeService,
            ICardService cardService,
            IEventoService eventoService)
        {
            _personajeService = personajeService;
            _cardService = cardService;
            _eventoService = eventoService;
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
    }
}
