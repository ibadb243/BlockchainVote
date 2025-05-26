using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<VoteChainDbContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("Postgres")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPollRepository, PollRepository>();
            services.AddScoped<IVoteRepository, VoteRepository>();
            services.AddScoped<IPendingVoteRepository, PendingVoteRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IBlockRepository, BlockRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "votechain_";
            });

            return services;
        }
    }
}
