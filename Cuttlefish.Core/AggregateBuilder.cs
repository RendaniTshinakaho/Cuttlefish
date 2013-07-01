using System;

namespace Cuttlefish
{
    public static class AggregateBuilder
    {
        public static T Get<T>(Guid id) where T : AggregateBase, new()
        {
            return (T) Activator.CreateInstance(typeof (T), Core.Instance.EventStore.GetEvents(id));
        }
    }
}