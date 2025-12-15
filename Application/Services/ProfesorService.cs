using System;
using System.Collections.Generic;
using System.Linq;
using Application.Abstractions;
using Contract.Requests;
using Contract.Responses;
using Domain.Entities;

namespace Application.Services
{
    public class ProfesorService : IProfesorService
    {
        private readonly IProfesorRepository _profesorRepository;
        private readonly IUsuarioService _usuarioService;

        public ProfesorService(
            IProfesorRepository profesorRepository,
            IUsuarioService usuarioService
        )
        {
            _profesorRepository = profesorRepository;
            _usuarioService = usuarioService;
        }

        public List<ProfesorResponse> GetAll()
        {
            var profesores = _profesorRepository
                .GetAll()
                .Where(p => p.Activo == true) // Solo activos
                .ToList();
            return profesores.Select(MapToProfesorResponse).ToList();
        }

        public List<ProfesorResponse> GetBySucursalId(int sucursalId)
        {
            var profesores = _profesorRepository
                .GetAll()
                .Where(p => p.SucursalId == sucursalId && p.Activo == true) // Por sucursal y activos
                .ToList();
            return profesores.Select(MapToProfesorResponse).ToList();
        }

        public ProfesorResponse? GetById(int id)
        {
            var profesor = _profesorRepository.GetById(id);
            if (profesor == null)
                return null;

            return MapToProfesorResponse(profesor);
        }

        public ProfesorResponse? Create(CreateProfesorRequest request)
        {
            if (
                request == null
                || string.IsNullOrWhiteSpace(request.Nombre)
                || string.IsNullOrWhiteSpace(request.Apellido)
                || string.IsNullOrWhiteSpace(request.Email)
            )
            {
                return null; // Validación de datos de entrada
            }

            if (_usuarioService.ExistsByEmail(request.Email))
            {
                return null; // Email ya existe
            }

            // Asignación de valores para propiedades obligatorias en la Entity base (Usuario.cs)
            var nuevoProfesor = new Profesor
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Dni = request.Dni,
                Email = request.Email,
                Telefono = request.Telefono,
                Activo = true,

                // CORRECCIONES CLAVE PARA EVITAR DbUpdateException (NOT NULL):

                // 1. PasswordHash es obligatorio en Usuario
                // Usamos un valor temporal (en producción, usaría una función de hashing)
                PasswordHash = "TEMPORARY_HASH_TO_FIX_DB_ERROR",

                // 2. Aseguramos el rol, crucial para el mapeo TPH/TPC
                Role = "Profesor",

                // 3. Clave Foránea obligatoria
                SucursalId = request.SucursalId,

                // 4. FechaNacimiento obligatoria (establecemos la fecha actual por defecto)
                FechaNacimiento = DateOnly.FromDateTime(DateTime.Now),

                // 5. Campos nullable por defecto (si la DB los requiere o son nullable)
                Genero = "N/A",
                Direccion = "N/A",
                Especialidad = request.Especialidad,
            };

            // Intentamos la creación
            if (_profesorRepository.Create(nuevoProfesor))
            {
                // Devolvemos el objeto recién creado con el ID
                return MapToProfesorResponse(nuevoProfesor);
            }

            return null; // Fallo en la inserción de la base de datos (Ej: SucursalId no existe)
        }

        public bool Update(int id, UpdateProfesorRequest request)
        {
            var profesor = _profesorRepository.GetById(id);
            if (profesor == null)
                return false;

            if (!string.IsNullOrWhiteSpace(request.Nombre))
                profesor.Nombre = request.Nombre;

            if (!string.IsNullOrWhiteSpace(request.Apellido))
                profesor.Apellido = request.Apellido;

            if (!string.IsNullOrWhiteSpace(request.Especialidad))
                profesor.Especialidad = request.Especialidad;

            if (!string.IsNullOrWhiteSpace(request.Telefono))
                profesor.Telefono = request.Telefono;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                if (request.Email != profesor.Email && _usuarioService.ExistsByEmail(request.Email))
                    return false;
                profesor.Email = request.Email;
            }

            if (request.FechaNacimiento.HasValue)
                profesor.FechaNacimiento = request.FechaNacimiento.Value;

            if (request.SucursalId.HasValue)
                profesor.SucursalId = request.SucursalId.Value;

            return _profesorRepository.Update(profesor);
        }

        public bool Delete(int id)
        {
            var profesor = _profesorRepository.GetById(id);
            if (profesor == null)
                return false;

            // Soft-Delete
            profesor.Activo = false;
            return _profesorRepository.Update(profesor);
        }

        private ProfesorResponse MapToProfesorResponse(Profesor profesor)
        {
            return new ProfesorResponse
            {
                Id = profesor.Id,
                Nombre = profesor.Nombre,
                Apellido = profesor.Apellido,
                Especialidad = profesor.Especialidad,
                Dni = profesor.Dni,
                Email = profesor.Email,
                Telefono = profesor.Telefono,
                FechaNacimiento = profesor.FechaNacimiento,
                SucursalId = profesor.SucursalId,
                Activo = profesor.Activo,
            };
        }
    }
}
