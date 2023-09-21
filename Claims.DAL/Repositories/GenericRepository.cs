using Claims.DAL.DataContext;
using Claims.DAL.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Claims.DAL.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, new()
    {
        private readonly AuditContext _auditContext;
        public GenericRepository(AuditContext auditContext)
        {
            _auditContext = auditContext;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _auditContext.AddAsync(entity);
            await _auditContext.SaveChangesAsync();
            return entity;
        }

        public async Task<List<TEntity>> AddRangeAsync(List<TEntity> entity)
        {
            await _auditContext.AddRangeAsync(entity);
            await _auditContext.SaveChangesAsync();
            return entity;
        }

        public async Task<int> DeleteAsync(TEntity entity)
        {
            _ = _auditContext.Remove(entity);
            return await _auditContext.SaveChangesAsync();
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default)
        {
            return await _auditContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(filter, cancellationToken);
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default)
        {
            return await (filter == null ? _auditContext.Set<TEntity>().ToListAsync(cancellationToken) : _auditContext.Set<TEntity>().Where(filter).ToListAsync(cancellationToken));
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _ = _auditContext.Update(entity);
            await _auditContext.SaveChangesAsync();
            return entity;
        }

        public async Task<List<TEntity>> UpdateRangeAsync(List<TEntity> entity)
        {
            _auditContext.UpdateRange(entity);
            await _auditContext.SaveChangesAsync();
            return entity;
        }
    }
}
