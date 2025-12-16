using System;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Security; // 🟢 Usamos la clase estática compartida
using Contract.Requests;
using Contract.Responses;
using Domain.Entities;
// 🔴 Ya no es necesario 'using System.Security.Cryptography;' aquí

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAlumnoRepository _alumnoRepository;
        private readonly IProfesorRepository _profesorRepository;
        private readonly IMembresiaService _membresiaService;
        private readonly IPlanRepository _planRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthService(
            IAlumnoRepository alumnoRepository,
            IProfesorRepository profesorRepository,
            IMembresiaService membresiaService,
            IPlanRepository planRepository,
            IUsuarioService usuarioService,
            IUsuarioRepository usuarioRepository
        )
        {
            _alumnoRepository = alumnoRepository;
            _profesorRepository = profesorRepository;
            _membresiaService = membresiaService;
            _planRepository = planRepository;
            _usuarioService = usuarioService;
            _usuarioRepository = usuarioRepository;
        }

        private DateTime? DateOnlyToNullableDateTime(DateOnly dateOnly)
        {
            if (dateOnly == default)
                return null;
            return dateOnly.ToDateTime(TimeOnly.MinValue);
        }

        // ====================== REGISTER ================================
        public async Task<AuthResponse?> Register(RegisterRequest request)
        {
            if (request.Role == "Alumno")
            {
                if (!_planRepository.IsActivo(request.PlanId))
                    return null;
                if (_usuarioService.ExistsByEmail(request.Email))
                    return null;

                var alumno = new Alumno
                {
                    Nombre = request.Nombre,
                    Apellido = request.Apellido,
                    Dni = request.Dni,
                    Email = request.Email,
                    Telefono = request.Telefono,
                    FechaNacimiento = request.FechaNacimiento,
                    Activo = true,
                    PasswordHash = PasswordHasher.Hash(request.Password), // 🟢 Usamos la clase estática
                    Role = "Alumno",
                    SucursalId = request.SucursalId,
                    Direccion = request.Direccion,
                    Genero = request.Genero,
                    Image = request.Image,
                    PlanId = request.PlanId,
                };

                if (!_alumnoRepository.Create(alumno))
                    return null;

                var membresiaRequest = new CreateMembresiaRequest
                {
                    AlumnoId = alumno.Id,
                    PlanId = request.PlanId,
                };

                var errorMembresia = _membresiaService.AsociarMembresia(membresiaRequest);

                if (errorMembresia != null)
                {
                    Console.WriteLine(
                        $"[AUTH SERVICE ERROR] Fallo al asociar membresía para Alumno ID {alumno.Id}: {errorMembresia}"
                    );
                    return null;
                }

                alumno.PlanId = request.PlanId;
                if (!_alumnoRepository.Update(alumno))
                {
                    Console.WriteLine(
                        $"[AUTH SERVICE ERROR] Fallo al actualizar PlanId para Alumno ID {alumno.Id}."
                    );
                    return null;
                }

                var nombrePlan = await _planRepository.GetNombrePlan(request.PlanId);

                return new AuthResponse
                {
                    Id = alumno.Id,
                    Nombre = alumno.Nombre,
                    Apellido = alumno.Apellido,
                    Role = alumno.Role,
                    Email = alumno.Email,
                    Dni = alumno.Dni,
                    TelNumber = alumno.Telefono,
                    FechaNacimiento = DateOnlyToNullableDateTime(alumno.FechaNacimiento),
                    Direccion = alumno.Direccion,
                    Genero = alumno.Genero,
                    SucursalId = alumno.SucursalId,
                    Image = alumno.Image,
                    PlanId = request.PlanId,
                    PlanName = nombrePlan ?? "Sin plan",
                };
            }
            else if (request.Role == "Profesor")
            {
                if (_usuarioService.ExistsByEmail(request.Email))
                    return null;

                var profesor = new Profesor
                {
                    Nombre = request.Nombre,
                    Apellido = request.Apellido,
                    Dni = request.Dni,
                    Email = request.Email,
                    Telefono = request.Telefono,
                    Activo = true,
                    PasswordHash = PasswordHasher.Hash(request.Password), // 🟢 Usamos la clase estática
                    Role = "Profesor",
                    SucursalId = request.SucursalId,
                    Direccion = request.Direccion,
                    Genero = request.Genero,
                    Image = request.Image,
                };

                if (!_profesorRepository.Create(profesor))
                    return null;

                return new AuthResponse
                {
                    Id = profesor.Id,
                    Nombre = profesor.Nombre,
                    Apellido = profesor.Apellido,
                    Role = profesor.Role,
                    Email = profesor.Email,
                    Dni = profesor.Dni,
                    TelNumber = profesor.Telefono,
                    FechaNacimiento = DateOnlyToNullableDateTime(profesor.FechaNacimiento),
                    Direccion = profesor.Direccion,
                    Genero = profesor.Genero,
                    SucursalId = profesor.SucursalId,
                    Image = profesor.Image,
                    PlanName = "Sin plan",
                };
            }

            return null;
        }

        // ====================== LOGIN ================================
        public async Task<AuthResponse?> Login(LoginRequest request)
        {
            var usuario = _usuarioService.GetWithPasswordByEmail(request.Email);
            if (usuario == null)
                return null;
            if (usuario.LockoutEnd.HasValue && usuario.LockoutEnd.Value > DateTime.UtcNow)
                return null;

            if (PasswordHasher.Verify(request.Password, usuario.PasswordHash)) // 🟢 Usamos la clase estática
            {
                if (usuario.FailedLoginAttempts > 0 || usuario.LockoutEnd.HasValue)
                {
                    usuario.FailedLoginAttempts = 0;
                    usuario.LockoutEnd = null;
                    _usuarioRepository.Update(usuario);
                }

                string? planNombre = null;
                if (usuario.PlanId.HasValue)
                    planNombre = await _planRepository.GetNombrePlan(usuario.PlanId.Value);

                return new AuthResponse
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Role = usuario.Role,
                    Email = usuario.Email,
                    Dni = usuario.Dni,
                    TelNumber = usuario.Telefono,
                    Genero = usuario.Genero,
                    FechaNacimiento = DateOnlyToNullableDateTime(usuario.FechaNacimiento),
                    Direccion = usuario.Direccion,
                    Estado = usuario.Activo ? "Activo" : "Inactivo",
                    PlanId = usuario.PlanId,
                    PlanName = planNombre ?? "Sin plan",
                    SucursalId = usuario.SucursalId,
                    Image = usuario.Image,
                };
            }

            usuario.FailedLoginAttempts++;
            if (usuario.FailedLoginAttempts >= 3)
                usuario.LockoutEnd = DateTime.UtcNow.AddMinutes(15);

            _usuarioRepository.Update(usuario);

            return null;
        }

        // 🔴 Se eliminan los métodos privados HashPassword y VerifyPassword, 
        // ya que la lógica fue movida a Application.Security.PasswordHasher.
    }
}