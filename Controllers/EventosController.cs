using ApiCatalogo_Galactico.Dtos;
using ApiCatalogo_Galactico.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo_Galactico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _service;
        private readonly IEstadisticaService _estadisticaService;
        private readonly ISimulacionService _simulacionService;

        public EventosController(
            IEventoService service,
            IEstadisticaService estadisticaService,
            ISimulacionService simulacionService)
        {
            _service = service;
            _estadisticaService = estadisticaService;
            _simulacionService = simulacionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoDto>>> Listar(
            [FromQuery] int? anioDesde,
            [FromQuery] int? anioHasta)
        {
            return Ok(await _service.ListarAsync(anioDesde, anioHasta));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EventoDto>> ObtenerPorId(int id)
        {
            var evento = await _service.ObtenerPorIdAsync(id);
            return evento is null ? NotFound() : Ok(evento);
        }

        [HttpGet("{id:int}/participantes")]
        public async Task<ActionResult<IEnumerable<PersonajeDto>>> ObtenerParticipantes(int id)
        {
            var participantes = await _service.ObtenerParticipantesAsync(id);
            return participantes is null ? NotFound() : Ok(participantes);
        }

        [HttpGet("{id:int}/mvp")]
        public async Task<ActionResult<MvpEventoDto>> ObtenerMvp(int id)
        {
            var (mvp, existe) = await _estadisticaService.ObtenerMvpAsync(id);
            if (!existe)
                return NotFound(new { mensaje = "Evento no encontrado." });

            if (mvp is null)
                return NotFound(new { mensaje = "Ningún participante del evento tiene una carta asociada." });

            return Ok(mvp);
        }

        [HttpGet("{id:int}/simular")]
        public async Task<ActionResult<ResultadoSimulacionDto>> Simular(int id)
        {
            var (resultado, error, existe) = await _simulacionService.SimularAsync(id);
            if (!existe)
                return NotFound(new { mensaje = "Evento no encontrado." });

            if (error is not null)
                return BadRequest(new { mensaje = error });

            return Ok(resultado);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventoDto>> Crear(EventoCrearDto dto)
        {
            var (creado, error) = await _service.CrearAsync(dto);
            if (error is not null)
                return BadRequest(new { mensaje = error });

            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado!.Id }, creado);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<EventoDto>> Actualizar(int id, EventoCrearDto dto)
        {
            var (actualizado, error, encontrado) = await _service.ActualizarAsync(id, dto);
            if (!encontrado)
                return NotFound();

            if (error is not null)
                return BadRequest(new { mensaje = error });

            return Ok(actualizado);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _service.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}
