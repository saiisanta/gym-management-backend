using Application.Abstractions;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories
{
    public class SalaRepository : BaseRepository<Sala>, ISalaRepository
    {
        public SalaRepository(GymDbContext context) : base(context)
        {
        }

        public List<Sala> GetBySucursalId(int sucursalId)
        {
            return GetByCriterial(s => s.SucursalId == sucursalId);
        }

        public bool Delete(int id)
        {
            var sala = _context.Set<Sala>().Find(id); 
            
            if (sala == null) return false;

            try
            {
                _context.Set<Sala>().Remove(sala);
                // ⚠️ Aquí es donde SaveChanges lanza la excepción si hay FKs.
                return _context.SaveChanges() > 0;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                // Manejo de la excepción de FK
                // Puedes loguear 'ex' aquí si lo necesitas.
                Console.WriteLine($"Error de integridad al eliminar Sala {id}: {ex.Message}");
                return false; 
            }
            catch (Exception ex)
            {
                // Captura cualquier otro error inesperado
                Console.WriteLine($"Error inesperado al eliminar Sala {id}: {ex.Message}");
                return false;
            }
        }
    }
}
