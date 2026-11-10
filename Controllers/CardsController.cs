using ApiCatalogo_Galactico.Dtos;
using ApiCatalogo_Galactico.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo_Galactico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardsController : ControllerBase
    {
        private readonly ICardService _service;

        public CardsController(ICardService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardDto>>> Listar()
        {
            return Ok(await _service.ListarAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CardDto>> ObtenerPorId(int id)
        {
            var card = await _service.ObtenerPorIdAsync(id);
            return card is null ? NotFound() : Ok(card);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CardDto>> Crear(CardCrearDto dto)
        {
            var (creada, error, conflicto) = await _service.CrearAsync(dto);
            if (conflicto)
                return Conflict(new { mensaje = error });

            if (error is not null)
                return BadRequest(new { mensaje = error });

            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada!.Id }, creada);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CardDto>> Actualizar(int id, CardCrearDto dto)
        {
            var (actualizada, error, encontrada, conflicto) = await _service.ActualizarAsync(id, dto);
            if (!encontrada)
                return NotFound();

            if (conflicto)
                return Conflict(new { mensaje = error });

            if (error is not null)
                return BadRequest(new { mensaje = error });

            return Ok(actualizada);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminada = await _service.EliminarAsync(id);
            return eliminada ? NoContent() : NotFound();
        }
    }
}
