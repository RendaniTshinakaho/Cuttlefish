using Cuttlefish.Common;
using Cuttlefish.Common.Exceptions;

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
            core.GetContainer().Configure(expression => expression.For<ICache>().Use<InMemoryDictionaryCache>());

            return core;
        }
    }
}