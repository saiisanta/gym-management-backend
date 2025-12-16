using Application.Abstractions;
using Application.Security; // 🟢 Importamos el namespace de seguridad
using Contract.Requests;
using Contract.Responses;
using Domain.Entities;

namespace Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMembresiaRepository _membresiaRepository;

        private string GetRoleName(int roleId)
        {
            return roleId switch
            {
                1 => "SuperAdministrador",
                2 => "Administrador",
                3 => "Recepcionista",
                4 => "Alumno",
                5 => "Profesor",
                _ => "Alumno",
            };
        }

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IMembresiaRepository membresiaRepository
        )
        {
            _usuarioRepository = usuarioRepository;
            _membresiaRepository = membresiaRepository;
        }

        public Usuario? GetByEmail(string email)
        {
            return _usuarioRepository.GetByEmail(email);
        }

        public Usuario? GetById(int id)
        {
            return _usuarioRepository.GetById(id);
        }

        public bool ExistsByEmail(string email)
        {
            return _usuarioRepository.ExistsByEmail(email);
        }

        public bool ExistsByDni(string dni)
        {
            return _usuarioRepository.ExistsByDni(dni);
        }

        public Usuario? GetWithPasswordByEmail(string email)
        {
            return _usuarioRepository.GetWithPasswordByEmail(email);
        }

        public bool Desactivar(int id)
        {
            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                return false;

            usuario.Activo = false;
            return _usuarioRepository.Update(usuario);
        }

        public Contract.Responses.UsuarioResponse? GetDtoByEmail(string email)
        {
            return _usuarioRepository.GetDtoByEmail(email);
        }

        public Contract.Responses.UsuarioResponse? GetDtoById(int id)
        {
            return _usuarioRepository.GetDtoById(id);
        }

        public List<Contract.Responses.UsuarioResponse> GetAllDtos()
        {
            return _usuarioRepository.GetAllDtos();
        }

        public List<Contract.Responses.UsuarioResponse> GetAllDtos(int? roleId, int? sucursalId)
        {
            var usuarios = _usuarioRepository.GetAllDtos();
            if (roleId.HasValue)
            {
                usuarios = usuarios.Where(u => u.RoleId == roleId.Value).ToList();
            }
            if (sucursalId.HasValue)
            {
                usuarios = usuarios.Where(u => u.SucursalId == sucursalId.Value).ToList();
            }

            return usuarios;
        }

        public (List<Contract.Responses.UsuarioResponse> Items, int Total) GetPagedDtos(
            int page,
            int pageSize,
            string? q = null
        )
        {
            return _usuarioRepository.GetPagedDtos(page, pageSize, q);
        }

        public bool Create(RegisterRequest request)
        {
            if (
                request == null
                || string.IsNullOrWhiteSpace(request.Email)
                || string.IsNullOrWhiteSpace(request.Password)
            )
            {
                return false;
            }

            if (_usuarioRepository.ExistsByEmail(request.Email))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(request.Dni) || request.Dni == "00000000")
            {
                // Generamos un DNI pseudo-único usando el tiempo actual (ticks)
                request.Dni = DateTime.Now.Ticks.ToString().Substring(0, 8);
            }
            if (_usuarioRepository.ExistsByDni(request.Dni))
            {
                return false;
            }
            var nuevoUsuario = new Usuario
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Email = request.Email,
                SucursalId = request.SucursalId,
                PasswordHash = PasswordHasher.Hash(request.Password), // 🟢 Llama al método estático
                Role = request.Role,
                Dni = request.Dni,
                Telefono = request.Telefono ?? string.Empty,
                Direccion = request.Direccion ?? string.Empty,
                FechaNacimiento =
                    request.FechaNacimiento != default(DateOnly)
                        ? request.FechaNacimiento
                        : new DateOnly(1900, 1, 1),
                Genero = request.Genero,
                Image = request.Image,
                PlanId = request.PlanId,
                Activo = true,
                FailedLoginAttempts = 0,
            };
            return _usuarioRepository.Create(nuevoUsuario);
        }

        public bool Delete(int id)
        {
            if (id == 1)
            {
                return false;
            }
            return _usuarioRepository.Delete(id);
        }

        public UsuarioResponse? Update(int id, UpdateUsuarioRequest request)
        {
            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                return null;

            // Mapear RoleId (frontend) -> Role string (DB)
            if (request.RoleId.HasValue)
            {
                usuario.Role = request.RoleId.Value switch
                {
                    1 => "SuperAdministrador", // frontend: 1 => superadmin
                    2 => "Administrador", // frontend: 2 => adminSucursal
                    3 => "Recepcionista", // frontend: 3 => recepcionista
                    4 => "Alumno", // frontend: 4 => cliente -> en la BD lo representás como "Alumno"
                    5 => "Profesor",
                    _ => usuario.Role,
                };
            }

            // PlanId: setear o borrar
            if (request.PlanId.HasValue)
            {
                usuario.PlanId = request.PlanId.Value;
            }
            else if (request.PlanId == null)
            {
                usuario.PlanId = null;
            }

            // Campos comunes
            if (!string.IsNullOrWhiteSpace(request.Nombre))
                usuario.Nombre = request.Nombre;
            if (!string.IsNullOrWhiteSpace(request.Apellido))
                usuario.Apellido = request.Apellido;
            if (!string.IsNullOrWhiteSpace(request.Telefono))
                usuario.Telefono = request.Telefono;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                if (
                    request.Email != usuario.Email
                    && _usuarioRepository.ExistsByEmail(request.Email)
                )
                    return null;
                usuario.Email = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.Genero))
                usuario.Genero = request.Genero;
            if (!string.IsNullOrWhiteSpace(request.Direccion))
                usuario.Direccion = request.Direccion;
            if (!string.IsNullOrWhiteSpace(request.Image))
                usuario.Image = request.Image;

            if (request.FechaNacimiento.HasValue)
                usuario.FechaNacimiento = request.FechaNacimiento.Value;
            if (request.SucursalId.HasValue)
                usuario.SucursalId = request.SucursalId.Value;

            var updated = _usuarioRepository.Update(usuario);
            if (!updated)
                return null;

            // DTO: devolver RoleId consistente con la convención del frontend
            return new UsuarioResponse
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Lastname = usuario.Apellido,
                Email = usuario.Email,
                Role = usuario.Role,
                RoleId = usuario.Role.ToLower() switch
                {
                    "superadministrador" => 1,
                    "administrador" => 2,
                    "admin" => 2,
                    "adminsucursal" => 2,
                    "recepcionista" => 3,
                    "alumno" => 4,
                    "cliente" => 4,
                    "profesor" => 5,
                    _ => 4, // fallback a "cliente" si el rol es desconocido
                },
                TelNumber = usuario.Telefono,
                Dni = usuario.Dni,
                Genero = usuario.Genero,
                FechaNacimiento = usuario.FechaNacimiento.ToString("yyyy-MM-dd"),
                Direccion = usuario.Direccion,
                Estado = usuario.Activo ? "activo" : "inactivo",
                Plan = usuario.PlanId.HasValue
                    ? _membresiaRepository.GetNombrePlan(usuario.PlanId.Value)
                    : null,
                SucursalId = usuario.SucursalId,
                Image = usuario.Image,
            };
        }
    }
}
