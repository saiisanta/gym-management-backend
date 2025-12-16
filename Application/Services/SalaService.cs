using Application.Abstractions;
using Contract.Requests;
using Contract.Responses;
using Domain.Entities;

namespace Application.Services
{
    public class SalaService : ISalaService
    {
        private readonly ISalaRepository _salaRepository;

        public SalaService(ISalaRepository salaRepository)
        {
            _salaRepository = salaRepository;
        }

        public SalaResponse Create(CreateSalaRequest request)
        {
            var sala = new Sala
            {
                SucursalId = request.SucursalId,
                Nombre = request.Nombre,
                Tipo = request.Tipo,
                Capacidad = request.Capacidad,
                Descripcion = request.Descripcion,
                Activa = true, // Por defecto
            };

            var createdSala = _salaRepository.Add(sala);

            return new SalaResponse
            {
                Id = createdSala.Id,
                SucursalId = createdSala.SucursalId,
                Nombre = createdSala.Nombre,
                Tipo = createdSala.Tipo,
                Capacidad = createdSala.Capacidad,
                Descripcion = createdSala.Descripcion,
                Activa = createdSala.Activa,
            };
        }

        public List<SalaResponse> GetAll()
        {
            var salas = _salaRepository.GetAll();
            return salas
                .Select(s => new SalaResponse
                {
                    Id = s.Id,
                    SucursalId = s.SucursalId,
                    Nombre = s.Nombre,
                    Tipo = s.Tipo,
                    Capacidad = s.Capacidad,
                    Descripcion = s.Descripcion,
                    Activa = s.Activa,
                })
                .ToList();
        }

        public List<SalaResponse> GetBySucursalId(int sucursalId)
        {
            var salas = _salaRepository.GetBySucursalId(sucursalId);
            return salas
                .Select(s => new SalaResponse
                {
                    Id = s.Id,
                    SucursalId = s.SucursalId,
                    Nombre = s.Nombre,
                    Tipo = s.Tipo,
                    Capacidad = s.Capacidad,
                    Descripcion = s.Descripcion,
                    Activa = s.Activa,
                })
                .ToList();
        }

        public SalaResponse? GetById(int id)
        {
            var sala = _salaRepository.GetById(id);
            if (sala == null)
                return null;

            return new SalaResponse
            {
                Id = sala.Id,
                SucursalId = sala.SucursalId,
                Nombre = sala.Nombre,
                Tipo = sala.Tipo,
                Capacidad = sala.Capacidad,
                Descripcion = sala.Descripcion,
                Activa = sala.Activa,
            };
        }

        public bool Update(int id, UpdateSalaRequest request)
        {
            var sala = _salaRepository.GetById(id);
            if (sala == null)
                return false;

            if (!string.IsNullOrWhiteSpace(request.Nombre))
                sala.Nombre = request.Nombre;

            if (!string.IsNullOrWhiteSpace(request.Tipo))
                sala.Tipo = request.Tipo;

            if (request.Capacidad.HasValue)
                sala.Capacidad = request.Capacidad.Value;

            if (!string.IsNullOrWhiteSpace(request.Descripcion))
                sala.Descripcion = request.Descripcion;

            return _salaRepository.Update(sala);
        }

        public bool Delete(int id)
        {
            return _salaRepository.Delete(id);
        }
    }
}
