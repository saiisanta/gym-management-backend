using System.Collections.Generic;
using System.Linq;
using Application.Abstractions;
using Contract.Responses;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly GymDbContext _context;

        public UsuarioRepository(GymDbContext context)
        {
            _context = context;
        }

        // ---------- MÉTODOS BÁSICOS ----------
        public Usuario? GetByEmail(string email) =>
            _context
                .Usuarios.Include(u => u.Plan)
                .Include(u => u.Sucursal)
                .FirstOrDefault(u => u.Email == email);

        public Usuario? GetById(int id) =>
            _context
                .Usuarios.Include(u => u.Plan)
                .Include(u => u.Sucursal)
                .FirstOrDefault(u => u.Id == id);

        public Usuario? GetWithPasswordByEmail(string email) => GetByEmail(email);

        public bool ExistsByEmail(string email) => _context.Usuarios.Any(u => u.Email == email);

        public bool ExistsByDni(string dni) =>
            _context.Usuarios.OfType<Alumno>().Any(a => a.Dni == dni);

        public bool IsActivo(int id)
        {
            var u = _context.Usuarios.Find(id);
            return u != null && u.Activo;
        }

        public bool Update(Usuario usuario)
        {
            try
            {
                _context.Usuarios.Update(usuario);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ---------- MÉTODOS DTO ----------
        private UsuarioResponse MapUsuarioToDto(Usuario u)
        {
            return new UsuarioResponse
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Lastname = u.Apellido,
                Email = u.Email,
                Role = u.Role,
                RoleId = u.Role.ToLower() switch
                {
                    "superadministrador" => 1,
                    "administrador" => 2,
                    "admin" => 2,
                    "adminsucursal" => 2,
                    "recepcionista" => 3,
                    "alumno" => 4,
                    "cliente" => 4,
                    "profesor" => 5,
                    _ => 4, // fallback a cliente
                },

                TelNumber = u.Telefono,
                Dni = u.Dni,
                Genero = u.Genero,
                Direccion = u.Direccion,
                FechaNacimiento = u.FechaNacimiento.ToString("yyyy-MM-dd"),
                Estado = u.Activo ? "activo" : "inactivo",
                Plan = u.Plan?.Nombre,
                SucursalId = u.SucursalId,
                Image = u.Image,
            };
        }

        public UsuarioResponse? GetDtoByEmail(string email)
        {
            var u = GetByEmail(email);
            return u == null ? null : MapUsuarioToDto(u);
        }

        public UsuarioResponse? GetDtoById(int id)
        {
            var u = GetById(id);
            return u == null ? null : MapUsuarioToDto(u);
        }

        public List<UsuarioResponse> GetAllDtos()
        {
            return _context
                .Usuarios.Include(u => u.Plan)
                .Include(u => u.Sucursal)
                .ToList()
                .Select(MapUsuarioToDto)
                .ToList();
        }

        public (List<UsuarioResponse> Items, int Total) GetPagedDtos(
            int page,
            int pageSize,
            string? q = null
        )
        {
            var query = _context
                .Usuarios.Include(u => u.Plan)
                .Include(u => u.Sucursal)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.ToLower();
                query = query.Where(u =>
                    u.Nombre.ToLower().Contains(q)
                    || u.Apellido.ToLower().Contains(q)
                    || u.Email.ToLower().Contains(q)
                );
            }

            var total = query.Count();
            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(MapUsuarioToDto)
                .ToList();

            return (items, total);
        }

        public bool HasMembresiaActiva(int alumnoId)
        {
            return _context.Membresias.Any(m =>
                m.AlumnoId == alumnoId
                && m.Activa
                && m.FechaFin >= DateOnly.FromDateTime(DateTime.Today)
            );
        }
    }
}
