using Application.Abstractions;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories
{
    public class ClaseRepository : BaseRepository<Clase>, IClaseRepository
    {
        private readonly GymDbContext _context;

        public ClaseRepository(GymDbContext context)
            : base(context)
        {
            _context = context;
        }

        public Clase? GetByIdWithDetails(int id)
        {
            return _context.Clases.FirstOrDefault(c => c.Id == id);
        }

        public List<Clase> GetByProfesorId(int profesorId)
        {
            return _context.Clases.Where(c => c.ProfesorId == profesorId && c.Activa).ToList();
        }

        public List<Clase> GetBySucursalId(int sucursalId)
        {
            return _context.Clases.Where(c => c.SucursalId == sucursalId && c.Activa).ToList();
        }

        public List<Clase> GetBySalaId(int salaId)
        {
            return _context.Clases.Where(c => c.SalaId == salaId && c.Activa).ToList();
        }

        public List<Clase> GetDisponiblesPorFecha(DateOnly fecha)
        {
            return _context.Clases.Where(c => c.Fecha == fecha && c.Activa).ToList();
        }

        public bool TieneConflictoHorario(
            int profesorId,
            DateOnly fecha,
            TimeOnly horaInicio,
            int duracionMinutos
        )
        {
            // 1. Calcular la Hora de Fin de la NUEVA CLASE (Esto se hace en C#)
            var horaFin = horaInicio.AddMinutes(duracionMinutos);

            // 2. Consulta de la base de datos (DB):
            // Filtramos solo por lo que EF Core puede traducir (ProfesorId, Fecha, Activa).
            // Usamos .AsEnumerable() para forzar la ejecución del resto de la consulta en C# (memoria).
            var clasesConflictivasPotenciales = _context
                .Clases.Where(c => c.ProfesorId == profesorId && c.Fecha == fecha && c.Activa)
                .AsEnumerable(); // <<<< ESTO RESUELVE EL ERROR DE TRADUCCIÓN >>>>

            // 3. Evaluación en C# (Client Evaluation):
            // Aplicamos la lógica compleja de solapamiento, que usa AddMinutes, en memoria.
            // Usaremos una lógica de solapamiento más simple y estándar:
            // (Inicio_Existente < Fin_Nueva) AND (Fin_Existente > Inicio_Nueva)
            return clasesConflictivasPotenciales.Any(c =>
                c.HoraInicio < horaFin && c.HoraInicio.AddMinutes(c.DuracionMinutos) > horaInicio
            );
        }
    }
}
