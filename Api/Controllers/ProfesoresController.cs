using Application.Services;
using Contract.Requests;
using Contract.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfesoresController : ControllerBase
    {
        private readonly IProfesorService _profesorService;
        private readonly IClaseService _claseService;

        public ProfesoresController(IProfesorService profesorService, IClaseService claseService)
        {
            _profesorService = profesorService;
            _claseService = claseService;
        }

        [HttpGet]
        [AllowAnonymous] // Permitir acceso público para obtener listado (o listado público)
        public ActionResult GetAll([FromQuery] int? sucursalId)
        {
            // var isAdmin = User.IsInRole("Administrador") || User.IsInRole("SuperAdministrador");
            var profesores = sucursalId.HasValue
                ? _profesorService.GetBySucursalId(sucursalId.Value)
                : _profesorService.GetAll();

            // Si es Admin, devuelve todos los detalles (incluyendo Email, DNI, Teléfono)
            // if (isAdmin)
            // {
            return Ok(profesores);
            //  }

            // Si no es Admin, devuelve solo la respuesta pública (sin DNI/Email sensibles)
            var profesoresPublicos = profesores
                .Select(p => new ProfesorPublicResponse
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    Activo = p.Activo,
                })
                .ToList();

            return Ok(profesoresPublicos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // Permitir acceso público a información básica de un profesor
        public ActionResult GetById(int id)
        {
            var profesor = _profesorService.GetById(id);
            if (profesor == null)
            {
                return NotFound("Profesor no encontrado.");
            }

            //      var isAdmin = User.IsInRole("Administrador") || User.IsInRole("SuperAdministrador");

            // Si es Admin, devuelve todos los detalles
            //   if (isAdmin)
            //    {
            return Ok(profesor);
            //  }

            // Si no es Admin, devuelve la respuesta pública
            var profesorPublico = new ProfesorPublicResponse
            {
                Id = profesor.Id,
                Nombre = profesor.Nombre,
                Apellido = profesor.Apellido,
                Activo = profesor.Activo,
            };

            return Ok(profesorPublico);
        }

        [HttpGet("{id}/clases")]
        [AllowAnonymous] //debe ser authorize
        public ActionResult<List<ClaseResponse>> GetClasesByProfesorId(int id)
        {
            var userIdClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier
            )?.Value;
            var isAdmin = User.IsInRole("Administrador") || User.IsInRole("SuperAdministrador");

            if (!isAdmin && userIdClaim != id.ToString())
            {
                return StatusCode(403, "No tiene permisos para ver las clases de otro profesor.");
            }

            var clases = _claseService.GetByProfesorId(id);
            return Ok(clases);
        }

        [HttpPut("{id}")]
        [AllowAnonymous] //debe ser authorize
        public IActionResult Update(int id, UpdateProfesorRequest request)
        {
            var userIdClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier
            )?.Value;
            var isAdmin = User.IsInRole("Administrador") || User.IsInRole("SuperAdministrador");

            if (!isAdmin && userIdClaim != id.ToString())
            {
                return StatusCode(403, "No tiene permisos para modificar este profesor.");
            }

            if (request == null)
                return BadRequest("La solicitud no puede ser nula.");

            var resultado = _profesorService.Update(id, request);
            if (!resultado)
                return BadRequest("No se pudo actualizar el profesor. Verifique los datos.");

            // CORRECCIÓN CLAVE: Devolver el objeto actualizado al front-end
            var profesorActualizado = _profesorService.GetById(id);
            return Ok(profesorActualizado);
        }

        [HttpPatch("{id}")]
        [AllowAnonymous] //debe ser authorize
        public IActionResult PartialUpdate(int id, UpdateProfesorRequest request)
        {
            return Update(id, request);
        }

        [HttpPost]
        [AllowAnonymous] //debe ser authorize
        public IActionResult Create([FromBody] CreateProfesorRequest request)
        {
            if (request == null)
                return BadRequest("La solicitud no puede estar vacía.");

            // CORRECCIÓN CLAVE: El service debe devolver el objeto creado (ProfesorResponse) o null
            var nuevoProfesor = _profesorService.Create(request);

            if (nuevoProfesor == null)
            {
                return BadRequest(
                    "No se pudo crear el profesor. Verifique los datos o el Email ya está registrado."
                );
            }

            // Usamos CreatedAtAction para devolver 201 Created y el objeto
            return CreatedAtAction(nameof(GetById), new { id = nuevoProfesor.Id }, nuevoProfesor);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous] //debe ser authorize
        public IActionResult Delete(int id)
        {
            var resultado = _profesorService.Delete(id);
            if (!resultado)
                return NotFound("Profesor no encontrado.");

            // El front-end solo necesita saber que fue exitoso (200 OK) para actualizar su lista
            return Ok(new { message = "Profesor eliminado exitosamente." });
        }
    }
}
