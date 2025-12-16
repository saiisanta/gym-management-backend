using Application.Services;
using Contract.Requests;
using Contract.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembresiasController : ControllerBase
    {
        private readonly IMembresiaService _membresiaService;

        public MembresiasController(IMembresiaService membresiaService)
        {
            _membresiaService = membresiaService;
        }

        [HttpGet]
        [AllowAnonymous] // Temporal: Permitir acceso sin autenticación para desarrollo/prueba.
        public ActionResult<List<MembresiaResponse>> GetByAlumno([FromQuery] int? alumnoId)
        {
            if (!alumnoId.HasValue)
                return BadRequest("El parámetro alumnoId es requerido.");

            // ... (Lógica de autorización comentada para desarrollo)

            var membresias = _membresiaService.GetByAlumnoId(alumnoId.Value);
            return Ok(membresias);
        }

        [HttpPost]
        [AllowAnonymous] // Temporal: Permitir a cualquier usuario no autenticado 'comprar' una membresía.
        public IActionResult Create([FromBody] CreateMembresiaRequest request)
        {
            if (request == null)
                return BadRequest("La solicitud no puede ser nula.");
            
            // CORRECCIÓN: Usar la nueva función del servicio que devuelve el mensaje de error.
            var error = _membresiaService.AsociarMembresia(request); 
            
            if (error != null)
                return BadRequest(error); // Devolvemos el mensaje de error exacto (400 Bad Request)

            return Ok(new { message = "Membresía creada exitosamente." });
        }

        [HttpPatch("{id}")]
        [AllowAnonymous] // Temporal: Permitir acceso sin autenticación.
        public IActionResult Update(int id, [FromBody] UpdateMembresiaRequest request)
        {
            if (request == null)
                return BadRequest("La solicitud no puede ser nula.");
            
            // COMENTARIO DE FUTURA REVISIÓN:
            /*
             * Este método debe estar protegido únicamente para administradores.
             * * 1. Reemplazar [AllowAnonymous] por [Authorize(Policy = "AdminPolicy")].
             */

            var resultado = _membresiaService.Update(id, request);
            if (!resultado)
                return NotFound("Membresía no encontrada.");

            return Ok(new { message = "Membresía actualizada exitosamente." });
        }
    }
}