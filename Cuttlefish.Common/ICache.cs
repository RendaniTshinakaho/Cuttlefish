using System;

namespace Cuttlefish.Common
{
    public interface ICache
    {
        void Cache<T>(T item) where T : IAggregate;
        T Fetch<T>(Guid id);
        object Fetch(Guid id, Type type);
    }
}