using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class PlanRepository : BaseRepository<Plan>, IPlanRepository
    {
        private readonly GymDbContext _context;

        public PlanRepository(GymDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<string?> GetNombrePlan(int planId)
        {
            return await _context
                .Planes.Where(p => p.Id == planId)
                .Select(p => p.Nombre)
                .FirstOrDefaultAsync();
        }

        public bool IsActivo(int planId)
        {
            var plan = _context.Planes.FirstOrDefault(p => p.Id == planId);
            return plan != null && plan.Activo;
        }
    }
}
