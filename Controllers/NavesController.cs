using ApiCatalogo_Galactico.Dtos;
using ApiCatalogo_Galactico.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo_Galactico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NavesController : ControllerBase
    {
        private readonly INaveService _service;

        public NavesController(INaveService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NaveDto>>> Listar()
        {
            return Ok(await _service.ListarAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NaveDto>> ObtenerPorId(int id)
        {
            var nave = await _service.ObtenerPorIdAsync(id);
            return nave is null ? NotFound() : Ok(nave);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NaveDto>> Crear(NaveCrearDto dto)
        {
            var (creada, error) = await _service.CrearAsync(dto);
            if (error is not null)
                return BadRequest(new { mensaje = error });

            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada!.Id }, creada);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<NaveDto>> Actualizar(int id, NaveCrearDto dto)
        {
            var (actualizada, error, encontrada) = await _service.ActualizarAsync(id, dto);
            if (!encontrada)
                return NotFound();

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
