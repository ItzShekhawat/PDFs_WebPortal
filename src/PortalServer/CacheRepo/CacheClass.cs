using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalServer.CacheRepo
{
    public class CacheClass : ICacheClass
    {
        private const string KEY = "PDF_KEY";
        private readonly IMemoryCache _memoryCache;

        public CacheClass(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Cache(string PDF_File)
        {
            // Filling Up the cache 
            var option = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(120),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(240)
            };
            _memoryCache.Set<string>(KEY, PDF_File, option);
        }


    }
}
