using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Data.DbContext;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repository.InterFases;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repository.Classes
{
    public class SessionRepository : GenericRepositoy<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext;

        public SessionRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainarAndCategoryAsync(CancellationToken ct = default)
        {
            return await _dbContext.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category).ToListAsync(ct);
        }

        public Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default) => _dbContext.Bookings.AsNoTracking().CountAsync(b => b.SessionId == sessionId, ct);

        public async Task<Session> GetSessionDetailsWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct = default)
      => await _dbContext.Sessions
          .AsNoTracking()
          .Include(s => s.Trainer)
          .Include(s => s.Category)
          .FirstOrDefaultAsync(i => i.Id == sessionId, ct);

    }
}
