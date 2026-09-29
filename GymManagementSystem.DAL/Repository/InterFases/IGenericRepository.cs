using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Data.Models;

namespace GymManagementSystem.DAL.Repository.InterFases
{
    public interface IGenericRepository<TEntity> where TEntity : BaseClass ,new()
    {


        //Get all TEntity
        Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default);
        //Get TEntity by id
        Task<TEntity?> GetByIdAsync(int id, bool tracking = false, CancellationToken ct = default);
        //Add new plan
        void Add(TEntity TEntity, CancellationToken cancellationToken = default);
        //Update TEntity       
        void Update(TEntity TEntity, CancellationToken cancellationToken = default);
        //Remove TEntity
        void Delete(TEntity TEntity, CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> pridicate, CancellationToken ct = default);

        Task<TEntity?> FirstOrDefultAsync(Expression<Func<TEntity, bool>> pridicate, bool tracking = false, CancellationToken ct = default);

        Task<int> CountAsync(Expression<Func<TEntity,bool>> predicate = null , CancellationToken ct = default);
    }
}
