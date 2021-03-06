﻿using Cuttlefish.Common;

namespace Cuttlefish.Caches.BasicInMemory
{
    public static class Extenstions
    {
        public static Core UseInMemoryCache(this Core core)
        {
            if (core.GetContainer().Model.HasImplementationsFor<ICache>())
            {
                throw new CacheAlreadyConfiguredException();
            }
            core.GetContainer()
                .Configure(expression => expression.For<ICache>().Singleton().Use<InMemoryDictionaryCache>());

            return core;
        }
    }
}