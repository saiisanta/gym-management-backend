using Application.Services;
using Contract.Requests;
using Contract.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalasController : ControllerBase
    {
        private readonly ISalaService _salaService;

        public SalasController(ISalaService salaService)
        {
            _salaService = salaService;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<SalaResponse> Create([FromBody] CreateSalaRequest request)
        {
            var nuevaSala = _salaService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = nuevaSala.Id }, nuevaSala);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<SalaResponse>> GetAll()
        {
            var salas = _salaService.GetAll();
            return Ok(salas);
        }

        [HttpGet("sucursal/{sucursalId}")]
        [AllowAnonymous]
        public ActionResult<List<SalaResponse>> GetBySucursalId(int sucursalId)
        {
            var salas = _salaService.GetBySucursalId(sucursalId);
            return Ok(salas);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<SalaResponse> GetById(int id)
        {
            var sala = _salaService.GetById(id);
            if (sala == null)
                return NotFound();
            return Ok(sala);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public IActionResult Update(int id, [FromBody] UpdateSalaRequest request)
        {
            var resultado = _salaService.Update(id, request);
            if (!resultado)
                return NotFound("Sala no encontrada.");

            return Ok(new { message = "Sala actualizada exitosamente." });
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public IActionResult Delete(int id)
        {
            var resultado = _salaService.Delete(id);

            if (!resultado)
            {
                return Conflict(
                    "No se pudo eliminar la sala. Puede tener clases u otros registros asociados que deben eliminarse primero."
                );
            }
            return Ok(new { message = "Sala eliminada físicamente exitosamente." });
        }
    }
}
