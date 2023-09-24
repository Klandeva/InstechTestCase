using Claims.DAL.Repositories;
using Claims.DAL.Repositories.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Claims.DAL.DataContext;

namespace Claims.DAL
{
    public static class DependencyInjection
    {
        public static void RegisterDALDependencies(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<AuditContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddSingleton<IClaimAuditRepository, ClaimAuditRepository>();
            services.AddSingleton<ICoverAuditRepository, CoverAuditRepository>();
        }
    }
}
