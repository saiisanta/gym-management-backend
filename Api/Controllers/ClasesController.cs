using Application.Services;
using Contract.Requests;
using Contract.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClasesController : ControllerBase
    {
        private readonly IClaseService _claseService;

        public ClasesController(IClaseService claseService)
        {
            _claseService = claseService;
        }

        [HttpGet]
        [AllowAnonymous] // OK
        public ActionResult<List<ClaseResponse>> GetAll([FromQuery] int? sucursalId)
        {
            var clases = sucursalId.HasValue
                ? _claseService.GetBySucursalId(sucursalId.Value)
                : _claseService.GetAll();
            return Ok(clases);
        }

        [HttpGet("fecha/{fecha}")]
        [AllowAnonymous] // <--- CORREGIDO: Antes era [Authorize]
        public ActionResult<List<ClaseResponse>> GetPorFecha(string fecha)
        {
            if (!DateOnly.TryParse(fecha, out var fechaParsed))
                return BadRequest("Formato de fecha inválido. Use YYYY-MM-DD.");

            var clases = _claseService.GetDisponiblesPorFecha(fechaParsed);
            return Ok(clases);
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // <--- CORREGIDO: Antes era [Authorize]
        public ActionResult<ClaseResponse> GetById(int id)
        {
            var clase = _claseService.GetById(id);
            if (clase == null)
                return NotFound();
            return Ok(clase);
        }

        [HttpPost]
        [AllowAnonymous] // OK (Lógica de seguridad comentada)
        public IActionResult Create(CreateClaseRequest request)
        {
            if (request == null)
                return BadRequest("La solicitud no puede ser nula.");

            // ... (Validaciones) ...

            var resultado = _claseService.Create(request);
            if (!resultado)
                return BadRequest("No se pudo crear la clase. Verifique los datos.");

            return Ok(new { message = "Clase creada exitosamente." });
        }

        [HttpPatch("{id}")]
        [AllowAnonymous] // OK (Lógica de seguridad eliminada)
        public IActionResult Update(int id, [FromBody] UpdateClaseRequest request)
        {
            // ... (Lógica de negocio limpia) ...

            var resultado = _claseService.Update(id, request);
            if (!resultado)
                return BadRequest("No se pudo actualizar la clase. Verifique los datos.");

            return Ok(new { message = "Clase actualizada exitosamente." });
        }

        [HttpDelete("{id}")]
        [AllowAnonymous] // OK
        public IActionResult Delete(int id)
        {
            // NOTA: La lógica de autenticación está presente, pero no bloquea al anónimo,
            // solo aplica la autorización si el usuario está autenticado. Para una
            // eliminación total de la seguridad en el Delete, elimina las líneas:
            /*
            bool isAuthenticated = User?.Identity?.IsAuthenticated ?? false;
            string? userIdClaim = null;
            bool isAdmin = false;

            if (isAuthenticated)
            {
                // ... (código que usa User.FindFirst y User.IsInRole) ...
            }
            */
            // Por ahora, funciona, ya que el 'else' no retorna 401.

            var profesorIdClase = _claseService.GetProfesorIdByClaseId(id);
            if (profesorIdClase == null)
            {
                return NotFound("Clase no encontrada.");
            }

            // ... (Lógica de autenticación que solo se ejecuta si hay token) ...
            
            var resultado = _claseService.Delete(id);
            if (!resultado)
                return NotFound("Clase no encontrada.");

            return Ok(new { message = "Clase eliminada exitosamente." });
        }
    }
}