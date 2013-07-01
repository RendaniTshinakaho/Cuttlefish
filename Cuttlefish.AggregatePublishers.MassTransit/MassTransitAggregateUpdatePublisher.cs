using System;
using Cuttlefish.Common;
using MassTransit;

namespace Cuttlefish.AggregatePublishers.MassTransit
{
    public class MassTransitAggregateUpdatePublisher : IDisposable, IAggregateUpdatePublisher
    {
        private IServiceBus _aggregateBus;

        public MassTransitAggregateUpdatePublisher(IServiceBus serviceBus)
        {
            _aggregateBus = serviceBus;
        }

        public void PublishUpdate(IAggregate aggregate)
        {
            _aggregateBus.Publish(aggregate);
        }

        public void Dispose()
        {
            if (_aggregateBus != null)
            {
                _aggregateBus.Dispose();
                _aggregateBus = null;
            }
        }
    }
}