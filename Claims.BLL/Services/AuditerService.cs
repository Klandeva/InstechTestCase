using Claims.BLL.Services.IServices;
using Claims.DAL.Entities;
using Claims.DAL.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Claims.BLL.Services
{
    public class AuditerService : IAuditerService
    {
        private readonly IClaimAuditRepository _claimAuditRepository;
        private readonly ICoverAuditRepository _coverAuditRepository;

        private readonly ChannelWriter<ClaimAudit> _claimAuditChannelWriter;
        private readonly ChannelWriter<CoverAudit> _coverAuditChannelWriter;

        public AuditerService(IClaimAuditRepository claimAuditRepository, ICoverAuditRepository coverAuditRepository, ChannelWriter<ClaimAudit> claimAuditChannelWriter, ChannelWriter<CoverAudit> coverAuditChannelWriter)
        {
            _claimAuditRepository = claimAuditRepository;
            _coverAuditRepository = coverAuditRepository;

            _claimAuditChannelWriter = claimAuditChannelWriter;
            _coverAuditChannelWriter = coverAuditChannelWriter;
        }

        public async Task AuditClaim(string id, string httpRequestType)
        {
            var claimAudit = new ClaimAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                ClaimId = id
            };

            await _claimAuditChannelWriter.WriteAsync(claimAudit);
        }

        public async Task AuditCover(string id, string httpRequestType)
        {
            var coverAudit = new CoverAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                CoverId = id
            };

            await _coverAuditChannelWriter.WriteAsync(coverAudit);
        }
    }
}
