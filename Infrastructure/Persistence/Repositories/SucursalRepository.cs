using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class SucursalRepository : BaseRepository<Sucursal>, ISucursalRepository
    {
        public SucursalRepository(GymDbContext context)
            : base(context) { }

        public List<Sucursal> GetActivas()
        {
            return GetByCriterial(s => s.Activa);
        }




        public int GetCantidadSalas(int sucursalId)
        {
            return _context.Salas.Count(s => s.SucursalId == sucursalId && s.Activa);
        }






        
    }
}
