using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Data.Models;

namespace GymManagementSystem.DAL.Repository.InterFases
{
    public interface ISessionRepository :IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainarAndCategoryAsync(CancellationToken ct = default);
        Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default);
        Task<Session> GetSessionDetailsWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct = default);

    }
}
