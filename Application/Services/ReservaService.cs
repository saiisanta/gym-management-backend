using Application.Abstractions;
using Contract.Requests;
using Contract.Responses;
using Domain.Entities;

namespace Application.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IAlumnoRepository _alumnoRepository;
        private readonly IClaseRepository _claseRepository;

        public ReservaService(
            IReservaRepository reservaRepository,
            IAlumnoRepository alumnoRepository,
            IClaseRepository claseRepository
        )
        {
            _reservaRepository = reservaRepository;
            _alumnoRepository = alumnoRepository;
            _claseRepository = claseRepository;
        }

        public ReservaResponse? Create(CreateReservaRequest request)
        {
            // Validar alumno
            var alumno = _alumnoRepository.GetById(request.AlumnoId);
            if (alumno == null)
                return null;

            // Validar clase
            var clase = _claseRepository.GetById(request.ClaseId);
            if (clase == null)
                return null;

            // VALIDAR CUPO DISPONIBLE
            var reservasExistentes = _reservaRepository.GetByClaseId(request.ClaseId);
            var cuposUsados = reservasExistentes.Count;
            var cupoMaximo = clase.Capacidad;

            if (cuposUsados >= cupoMaximo)
            {
                throw new InvalidOperationException("La clase está llena.");
            }

            // Validar reserva duplicada
            var reservaExistente = _reservaRepository.GetByAlumnoYClase(
                request.AlumnoId,
                request.ClaseId
            );
            if (reservaExistente != null)
                return null;

            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

            var nuevaReserva = new Reserva
            {
                AlumnoId = request.AlumnoId,
                ClaseId = request.ClaseId,
                FechaReserva = hoy,
                Activo = true,
            };

            _reservaRepository.Create(nuevaReserva);

            return new ReservaResponse
            {
                Id = nuevaReserva.Id,
                AlumnoId = nuevaReserva.AlumnoId,
                ClaseId = nuevaReserva.ClaseId,
                FechaReserva = hoy.ToString("yyyy-MM-dd"),
                CreatedAt = DateTime.UtcNow.ToString("o"),
                Estado = "confirmada",
                Activo = nuevaReserva.Activo,
            };
        }

        public List<ReservaResponse> GetByAlumnoId(int alumnoId)
        {
            var reservas = _reservaRepository.GetByAlumnoId(alumnoId);

            return reservas
                .Select(r => new ReservaResponse
                {
                    Id = r.Id,
                    AlumnoId = r.AlumnoId,
                    ClaseId = r.ClaseId,
                    FechaReserva = r.FechaReserva.ToString("yyyy-MM-dd"),
                    Activo = r.Activo,
                })
                .ToList();
        }

        public List<ReservaResponse> GetByClaseId(int claseId)
        {
            var reservas = _reservaRepository.GetByClaseId(claseId);

            return reservas
                .Select(r => new ReservaResponse
                {
                    Id = r.Id,
                    AlumnoId = r.AlumnoId,
                    ClaseId = r.ClaseId,
                    FechaReserva = r.FechaReserva.ToString("yyyy-MM-dd"),
                    Activo = r.Activo,
                })
                .ToList();
        }

        public int? GetAlumnoIdByReservaId(int reservaId)
        {
            var reserva = _reservaRepository.GetById(reservaId);
            return reserva?.AlumnoId;
        }

        public bool Delete(int id)
        {
            var reserva = _reservaRepository.GetById(id);
            if (reserva == null)
                return false;

            return _reservaRepository.Delete(reserva);
        }
    }
}
