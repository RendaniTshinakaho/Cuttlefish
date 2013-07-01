using System;
using Cuttlefish.Common;
using MassTransit;
using NUnit.Framework;

namespace Cuttlefish.AggregatePublishers.MassTransit
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void CanPublishToQueue()
        {
            using (var test = new MassTransitAggregateUpdatePublisher(ServiceBusFactory.New(sbc =>
                {
                    sbc.UseRabbitMq();
                    sbc.ReceiveFrom("rabbitmq://localhost/AggregateUpdatePublisherTests");
                })))
            {
                test.PublishUpdate(new TestAggregate());
            }
        }
    }

    public class TestAggregate : IAggregate
    {
        public TestAggregate()
        {
            AggregateIdentity = Guid.NewGuid();
            TypeName = GetType().Name;
        }

        public Guid AggregateIdentity { get; set; }
        public string TypeName { get; private set; }
    }
}