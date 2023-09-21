using Claims.BLL.Services.IServices;
using Claims.DAL.Entities;
using Claims.DAL.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.BLL.Services
{
    public class AuditerService : IAuditerService
    {
        private readonly IClaimAuditRepository _claimAuditRepository;
        private readonly ICoverAuditRepository _coverAuditRepository;

        public AuditerService(IClaimAuditRepository claimAuditRepository, ICoverAuditRepository coverAuditRepository)
        {
            _claimAuditRepository = claimAuditRepository;
            _coverAuditRepository = coverAuditRepository;
        }

        public async Task AuditClaim(string id, string httpRequestType)
        {
            var claimAudit = new ClaimAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                ClaimId = id
            };

            await _claimAuditRepository.AddAsync(claimAudit);
        }

        public async Task AuditCover(string id, string httpRequestType)
        {
            var coverAudit = new CoverAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                CoverId = id
            };

            await _coverAuditRepository.AddAsync(coverAudit);
        }
    }
}
