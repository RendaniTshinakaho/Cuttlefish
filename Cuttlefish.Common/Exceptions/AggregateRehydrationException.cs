using System;

namespace Cuttlefish.Common
{
    public class AggregateRehydrationException : Exception
    {
        public AggregateRehydrationException(IAggregate aggregateBase, EventExecutionException eventExecutionException)
            : base("Failed to reydrate aggregate", eventExecutionException)
        {
            AggregateBase = aggregateBase;
        }

        public IAggregate AggregateBase { get; private set; }
    }
}