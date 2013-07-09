using System;
using Cuttlefish.Common;

namespace Cuttlefish
{
    public class AggregateRehydrationException : Exception
    {
        public IAggregate AggregateBase { get; private set; }

        public AggregateRehydrationException(IAggregate aggregateBase, EventExecutionException eventExecutionException)
            : base("Failed to reydrate aggregate", eventExecutionException)
        {
            AggregateBase = aggregateBase;
        }
    }
}