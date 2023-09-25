using Claims.DAL.Entities;
using Claims.DAL.Repositories;
using Claims.DAL.Repositories.IRepositories;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Claims.BLL.Services
{
    public class ClaimAuditReaderService : BackgroundService
    {
        private readonly ChannelReader<ClaimAudit> _channelReader;
        private readonly IClaimAuditRepository _claimAuditRepository;
        public ClaimAuditReaderService(ChannelReader<ClaimAudit> channelReader, IClaimAuditRepository claimAuditRepository) 
        {
            _channelReader = channelReader;
            _claimAuditRepository = claimAuditRepository;
        }

        /// <summary>
        /// Get ClaimAudit object from Channel queue and insert it into ClaimAudits SQLServer db table
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ClaimAudit claimAudit = await _channelReader.ReadAsync(stoppingToken);
                if (claimAudit != null) 
                { 
                    await _claimAuditRepository.AddAsync(claimAudit);
                }
            }
        }
    }
}
