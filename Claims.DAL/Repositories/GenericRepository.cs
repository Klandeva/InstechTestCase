using Claims.DAL.DataContext;
using Claims.DAL.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Claims.DAL.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, new()
    {
        private readonly IServiceProvider _serviceProvider;
        public GenericRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var auditContext = scope.ServiceProvider.GetService<AuditContext>();

                await auditContext.AddAsync(entity);
                await auditContext.SaveChangesAsync();
                return entity;
            }

        }

        public async Task<List<TEntity>> AddRangeAsync(List<TEntity> entity)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var auditContext = scope.ServiceProvider.GetService<AuditContext>();
                await auditContext.AddRangeAsync(entity);
                await auditContext.SaveChangesAsync();
                return entity;
            }
        }

        public async Task<int> DeleteAsync(TEntity entity)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var auditContext = scope.ServiceProvider.GetService<AuditContext>();
                _ = auditContext.Remove(entity);
                return await auditContext.SaveChangesAsync();
            }
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var auditContext = scope.ServiceProvider.GetService<AuditContext>();
                return await auditContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(filter, cancellationToken);
            }
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var auditContext = scope.ServiceProvider.GetService<AuditContext>();
                return await (filter == null ? auditContext.Set<TEntity>().ToListAsync(cancellationToken) : auditContext.Set<TEntity>().Where(filter).ToListAsync(cancellationToken));
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var auditContext = scope.ServiceProvider.GetService<AuditContext>();
                _ = auditContext.Update(entity);
                await auditContext.SaveChangesAsync();
                return entity;
            }
        }

        public async Task<List<TEntity>> UpdateRangeAsync(List<TEntity> entity)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var auditContext = scope.ServiceProvider.GetService<AuditContext>();
                auditContext.UpdateRange(entity);
                await auditContext.SaveChangesAsync();
                return entity;
            }
        }
    }
}
