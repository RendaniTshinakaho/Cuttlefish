using Cuttlefish.AggregatePublishers.MassTransit;
using Cuttlefish.Common;
using Cuttlefish.EventStorage.NEventStore;
using MassTransit;
using NUnit.Framework;

namespace Cuttlefish.Tests
{
    [TestFixture]
    public class AggregateUpdatePublisherTests
    {
        [Test]
        public void CanSetupAggregateUpdatePublisher()
        {
            Core.Reset();
            Core.Configure()
                .UseInMemoryEventStore()
                .UseMassTransitAggregateUpdatePublisher(ServiceBusFactory.New(sbc =>
                    {
                        sbc.UseRabbitMq();
                        sbc.ReceiveFrom("rabbitmq://127.0.0.1/AggregateUpdatePublisherTests");
                    }))
                .Done();

            Assert.That(Core.Instance.GetContainer().Model.HasImplementationsFor<IAggregateUpdatePublisher>(), Is.True);
        }
    }
}