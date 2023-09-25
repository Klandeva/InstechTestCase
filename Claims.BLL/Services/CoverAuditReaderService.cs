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
    public class CoverAuditReaderService : BackgroundService
    {
        private readonly ChannelReader<CoverAudit> _channelReader;
        private readonly ICoverAuditRepository _coverAuditRepository;

        public CoverAuditReaderService(ChannelReader<CoverAudit> channelReader, ICoverAuditRepository coverAuditRepository)
        {
            _channelReader = channelReader;
            _coverAuditRepository = coverAuditRepository;
        }

        /// Get CoverAudit object from Channel queue and insert it into CoverAudits SQLServer db table
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                CoverAudit coverAudit = await _channelReader.ReadAsync(stoppingToken);
                if (coverAudit != null)
                {
                    await _coverAuditRepository.AddAsync(coverAudit);
                }
            }
        }
    }
}
