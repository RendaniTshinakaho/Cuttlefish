using Cuttlefish.Common;

namespace Cuttlefish
{
    public class EventRouter
    {
        /// <summary>
        ///     Used by services to trigger events to be stored in the event store. Should be refactored.
        /// </summary>
        public static void FireEventOnAggregate<T>(IEvent @event) where T : AggregateBase, new()
        {
            var aggregate = AggregateBuilder.Get<T>(@event.AggregateIdentity);
            FireEventOnAggregate(aggregate, @event);
        }

        /// <summary>
        ///     Used by services to trigger events to be stored in the event store. Should be refactored.
        /// </summary>
        public static void FireEventOnAggregate(AggregateBase aggregate, IEvent @event)
        {
            try
            {
                aggregate.InvokeEvent(@event);
                Core.Instance.EventStore.AddEvent(@event);
            }
            catch
            {
                throw new EventExecutionException(aggregate, @event);
            }
        }
    }
}