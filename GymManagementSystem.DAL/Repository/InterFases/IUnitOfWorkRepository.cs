using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Data.Models;

namespace GymManagementSystem.DAL.Repository.InterFases
{
    public interface IUnitOfWorkRepository
    {
        IGenericRepository<T> GetRepository<T>() where T : BaseClass,new();
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        public ISessionRepository _sessionRepository { get; }
    }
}
