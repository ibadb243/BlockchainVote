using Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IBlockService, BlockService>();
            services.AddScoped<IMerkleTreeService, MerkleTreeService>();
            services.AddScoped<ITransactionConverterService, TransactionConverterService>();

            return services;
        }
    }
}
