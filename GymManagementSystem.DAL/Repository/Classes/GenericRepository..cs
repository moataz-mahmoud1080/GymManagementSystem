using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Data.DbContext;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repository.InterFases;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repository.Classes
{
    public class GenericRepositoy<TEntity> : IGenericRepository<TEntity> where TEntity : BaseClass, new()
    {
        private readonly GymDbContext _dbContext;
        private readonly DbSet<TEntity> _set;

        public GenericRepositoy(GymDbContext dbContext) 
        {
            _dbContext = dbContext;
            _set = dbContext.Set<TEntity>();
        }

        public void Add(TEntity TEntity, CancellationToken ct= default)=> _set.Add(TEntity);
        public void Delete(TEntity TEntity, CancellationToken ct= default)=> _set.Remove(TEntity);
        public void Update(TEntity TEntity, CancellationToken ct = default)=> _set.Update(TEntity);


        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
            return await query.ToListAsync(ct);
        }
        public async Task<TEntity?> FirstOrDefultAsync(Expression<Func<TEntity, bool>> pridicate, bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
            return await query.FirstOrDefaultAsync(pridicate, ct);
        }
        public async Task<TEntity?> GetByIdAsync(int id, bool tracking = false, CancellationToken ct = default)
        {
            return await _set.FindAsync(id, ct);
        }
        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> pridicate, CancellationToken ct = default)
        {
            return  _set.AsNoTracking().AnyAsync(pridicate, ct);
        }

        public  Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken ct = default)
        => predicate is null ? _set.AsNoTracking().CountAsync(ct) : _set.AsNoTracking().CountAsync(predicate, ct);
    }

}
