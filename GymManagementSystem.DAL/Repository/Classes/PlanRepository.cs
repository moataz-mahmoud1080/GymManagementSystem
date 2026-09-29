using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Data.DbContext;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repository.InterFases;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repository.Classes
{
    public class PlanRepository :GenericRepositoy<Plan>, IPlanRepositoy
    {
        private readonly GymDbContext _context;

        public PlanRepository(GymDbContext context ) : base( context ) 
        { 
            _context = context;
        }

        public async Task<IEnumerable<Plan>> GetAllAsync(bool tracking = false, CancellationToken cancellationToken = default)
        {
            IQueryable<Plan> query = tracking ? _context.Plans : _context.Plans.AsNoTracking();
            return await query.ToListAsync();
        }
        public async Task<Plan?> GetByIdAsync(int id, bool tracking = false, CancellationToken cancellationToken = default)
        {
            return await _context.Plans.FindAsync(id, cancellationToken);
        }
        public async Task<int> AddAsync(Plan plan, CancellationToken cancellationToken =default)
        {
            _context.Plans.Add(plan);
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> DeleteAsync(Plan plan, CancellationToken cancellationToken = default)
        {
            _context.Plans.Remove(plan);
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> UpdateAsync(Plan plan, CancellationToken cancellationToken = default)
        {
            _context.Plans.Update(plan);
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
