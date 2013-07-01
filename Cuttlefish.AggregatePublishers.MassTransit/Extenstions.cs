using System;
using Cuttlefish.Common;
using Cuttlefish.Common.Exceptions;
using MassTransit;

namespace Cuttlefish.AggregatePublishers.MassTransit
{
    public static class Extenstions
    {
        public static Core UseMassTransitAggregateUpdatePublisher(this Core core, IServiceBus serviceBus)
        {
            if (serviceBus == null)
            {
                throw new ArgumentNullException("serviceBus");
            }

            if (core.GetContainer().Model.HasImplementationsFor<IAggregateUpdatePublisher>())
            {
                throw new EventStoreAlreadyConfiguredException();
            }

            core.GetContainer()
                .Configure(
                    cfg =>
                    cfg.For<IAggregateUpdatePublisher>()
                       .Singleton()
                       .Use<MassTransitAggregateUpdatePublisher>()
                       .Ctor<IServiceBus>()
                       .Is(serviceBus));
            return core;
        }
    }
}