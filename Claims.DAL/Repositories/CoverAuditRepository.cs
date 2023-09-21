using Claims.DAL.DataContext;
using Claims.DAL.Entities;
using Claims.DAL.Repositories.IRepositories;

namespace Claims.DAL.Repositories
{
    public class CoverAuditRepository : GenericRepository<CoverAudit>, ICoverAuditRepository
    {
        public CoverAuditRepository(AuditContext auditContext) : base(auditContext)
        {
        }
    }
}
