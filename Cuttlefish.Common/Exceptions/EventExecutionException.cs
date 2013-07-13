using System;

namespace Cuttlefish.Common
{
    public class EventExecutionException : Exception
    {
        public EventExecutionException(object aggregateBase, object @event)
        {
            AggregateBase = aggregateBase;
            Event = @event;
        }

        public object AggregateBase { get; private set; }
        public object Event { get; private set; }
    }
}