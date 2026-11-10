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

    }
}
