using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options = null);
        Task<T?> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}
