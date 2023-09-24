using Claims.BLL.Services;
using Claims.BLL.Services.IServices;
using Claims.DAL.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Threading.Channels;

namespace Claims.BLL
{
    public static class DependencyInjection
    {
        public static void RegisterBLLDependencies(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddSingleton(InitializeClaimCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
            services.AddSingleton(InitializeCoverCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());

            services.AddSingleton<ICoversService, CoverService>();

            services.AddScoped<IAuditerService, AuditerService>();

            // Claim Channel
            services.AddSingleton(Channel.CreateUnbounded<ClaimAudit>(new UnboundedChannelOptions() { SingleReader = true }));
            services.AddSingleton(svc => svc.GetRequiredService<Channel<ClaimAudit>>().Reader);
            services.AddSingleton(svc => svc.GetRequiredService<Channel<ClaimAudit>>().Writer);

            // Cover Channel
            services.AddSingleton(Channel.CreateUnbounded<CoverAudit>(new UnboundedChannelOptions() { SingleReader = true }));
            services.AddSingleton(svc => svc.GetRequiredService<Channel<CoverAudit>>().Reader);
            services.AddSingleton(svc => svc.GetRequiredService<Channel<CoverAudit>>().Writer);

            services.AddHostedService<ClaimAuditReaderService>();
            services.AddHostedService<CoverAuditReaderService>();

            static async Task<ICosmosDbService<Claim>> InitializeClaimCosmosClientInstanceAsync(IConfigurationSection configurationSection)
            {
                string account = configurationSection.GetSection("Account").Value;
                string key = configurationSection.GetSection("Key").Value;
                string databaseName = configurationSection.GetSection("ClaimDbName").Value;
                string containerName = configurationSection.GetSection("ClaimContainerName").Value;

                CosmosClient client = new CosmosClient(account, key);

                DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
                await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

                return new CosmosDbService<Claim>(client, databaseName, containerName);
            }

            static async Task<ICosmosDbService<Cover>> InitializeCoverCosmosClientInstanceAsync(IConfigurationSection configurationSection)
            {
                string account = configurationSection.GetSection("Account").Value;
                string key = configurationSection.GetSection("Key").Value;
                string databaseName = configurationSection.GetSection("CoverDbName").Value;
                string containerName = configurationSection.GetSection("CoverContainerName").Value;

                CosmosClient client = new CosmosClient(account, key);

                DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
                await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

                return new CosmosDbService<Cover>(client, databaseName, containerName);
            }
        }
    }
}
