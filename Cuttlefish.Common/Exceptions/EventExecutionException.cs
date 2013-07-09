using System;

namespace Cuttlefish
{
    public class EventExecutionException : Exception
    {
        public object AggregateBase { get; private set; }
        public object Event { get; private set; }

        public EventExecutionException(object aggregateBase, object @event)
        {
            AggregateBase = aggregateBase;
            Event = @event;
        }
    }
}