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
        private readonly INaveService _naveService;

        public EventosController(IEventoService service, INaveService naveService)
        {
            _service = service;
            _naveService = naveService;
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

        [HttpGet("{id:int}/despliegue-flota")]
        public async Task<ActionResult<DespliegueFlotaEventoDto>> ObtenerDespliegueFlota(int id)
        {
            var reporte = await _naveService.ObtenerDespliegueFlotaAsync(id);
            return reporte is null ? NotFound(new { mensaje = "Evento no encontrado." }) : Ok(reporte);
        }

        [HttpGet("{id:int}/refuerzos-disponibles")]
        public async Task<ActionResult<IEnumerable<RefuerzoDisponibleDto>>> ObtenerRefuerzosDisponibles(
            int id,
            [FromQuery] int poderMinimo = 120)
        {
            if (poderMinimo < 1 || poderMinimo > 200)
                return BadRequest(new { mensaje = "El poderMinimo debe estar entre 1 y 200." });

            var refuerzos = await _naveService.ObtenerRefuerzosDisponiblesAsync(id, poderMinimo);
            return refuerzos is null ? NotFound(new { mensaje = "Evento no encontrado." }) : Ok(refuerzos);
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
