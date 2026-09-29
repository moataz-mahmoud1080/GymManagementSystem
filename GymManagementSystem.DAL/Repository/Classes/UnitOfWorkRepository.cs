using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Data.DbContext;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repository.InterFases;

namespace GymManagementSystem.DAL.Repository.Classes
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        private readonly GymDbContext _dbContext;
        private readonly Dictionary<string, object> _repositories = [];
        public ISessionRepository _sessionRepository { get; }

        public UnitOfWorkRepository(GymDbContext dbContext ,ISessionRepository sessionRepository )
        {
            _dbContext = dbContext;
            _sessionRepository = sessionRepository;
        }


        public IGenericRepository<T> GetRepository<T>() where T : BaseClass, new()
        {
            var typeName = typeof(T).Name;
            if (_repositories.TryGetValue(typeName, out object? value))
                return (IGenericRepository<T>)value;

            else
            {
                var repo = new GenericRepositoy<T>(_dbContext);
                _repositories [typeName]= repo;
                return repo;
            }
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)=>_dbContext.SaveChangesAsync(ct);

    }
}
