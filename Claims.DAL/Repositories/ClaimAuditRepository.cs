using Claims.DAL.DataContext;
using Claims.DAL.Entities;
using Claims.DAL.Repositories.IRepositories;


namespace Claims.DAL.Repositories
{
    public class ClaimAuditRepository : GenericRepository<ClaimAudit>, IClaimAuditRepository
    {
        public ClaimAuditRepository(IServiceProvider serviceProvider) : base(serviceProvider) 
        { 
        }
    }
}
