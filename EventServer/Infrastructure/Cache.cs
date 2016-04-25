using Microsoft.Practices.EnterpriseLibrary.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventServer.Infrastructure
{
    public class Cache
    {
        private static ICacheManager instance;

        private Cache() { }

        public static ICacheManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = CacheFactory.GetCacheManager();
                }
                return instance;
            }
        }
    }
}