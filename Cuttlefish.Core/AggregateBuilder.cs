using System;
using Fasterflect;

namespace Cuttlefish
{
    public static class AggregateBuilder
    {
        public static T Get<T>(Guid aggregateIdentity) where T : AggregateBase, new()
        {
            return (T) Activator.CreateInstance(typeof (T), Core.Instance.EventStore.GetEvents(aggregateIdentity));
        }

        public static AggregateBase Get(Guid aggregateIdentity, Type typeOfAggregate)
        {
            return (AggregateBase) typeOfAggregate.CreateInstance(Core.Instance.EventStore.GetEvents(aggregateIdentity));
        }
    }
}