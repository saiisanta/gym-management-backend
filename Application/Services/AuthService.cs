using Application.Abstractions;
using Contract.Requests;
using Contract.Responses;
using Domain.Entities;
using System.Security.Cryptography;
using System;

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
            IUsuarioRepository usuarioRepository)
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
            if (dateOnly == default(DateOnly))
            {
                return null;
            }
            return dateOnly.ToDateTime(TimeOnly.MinValue);
        }

        public AuthResponse? Register(RegisterRequest request)
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
                    PasswordHash = HashPassword(request.Password),
                    Role = "Alumno",
                    SucursalId = request.SucursalId, 
                    Direccion = request.Direccion, 
                    Genero = request.Genero, 
                    Image = request.Image
                };

                if (!_alumnoRepository.Create(alumno)) return null;

                var membresiaRequest = new CreateMembresiaRequest
                {
                    AlumnoId = alumno.Id,
                    PlanId = request.PlanId
                };

                if (!_membresiaService.AsociarMembresia(membresiaRequest))
                    return null;

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
                    Plan = request.PlanId 
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
                    PasswordHash = HashPassword(request.Password),
                    Role = "Profesor",
                    SucursalId = request.SucursalId,
                    Direccion = request.Direccion,
                    Genero = request.Genero,
                    Image = request.Image
                };

                if (!_profesorRepository.Create(profesor)) return null;

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
                    Plan = null 
                };
            }

            return null;
        }

        public AuthResponse? Login(LoginRequest request)
        {
            var usuario = _usuarioService.GetWithPasswordByEmail(request.Email);
            if (usuario == null)
                return null;

            if (usuario.LockoutEnd.HasValue && usuario.LockoutEnd.Value > DateTime.UtcNow)
            {
                return null;
            }

            if (VerifyPassword(request.Password, usuario.PasswordHash))
            {
                if (usuario.FailedLoginAttempts > 0 || usuario.LockoutEnd.HasValue)
                {
                    usuario.FailedLoginAttempts = 0;
                    usuario.LockoutEnd = null;
                    _usuarioRepository.Update(usuario);
                }

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
                    Plan = usuario.PlanId,
                    SucursalId = usuario.SucursalId, 
                    Image = usuario.Image
                };
            }

            usuario.FailedLoginAttempts++;

            if (usuario.FailedLoginAttempts >= 3)
            {
                usuario.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            }

            _usuarioRepository.Update(usuario);

            return null;
        }

        private string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            byte[] hashBytes = Convert.FromBase64String(hash);

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            byte[] testHash = pbkdf2.GetBytes(32);

            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != testHash[i])
                    return false;
            }

            return true;
        }
    }
}