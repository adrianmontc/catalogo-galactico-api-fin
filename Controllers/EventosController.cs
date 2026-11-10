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

        public EventosController(IEventoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoDto>>> Listar(
            [FromQuery] int? anioDesde,
            [FromQuery] int? anioHasta)
        {
            return Ok(await _service.ListarAsync(anioDesde, anioHasta));
        }
    }
}
