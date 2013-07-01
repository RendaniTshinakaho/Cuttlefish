using Cuttlefish.AggregatePublishers.MassTransit;
using Cuttlefish.Caches.BasicInMemory;
using Cuttlefish.EventStorage.NEventStore;
using MassTransit;
using NUnit.Framework;

namespace Cuttlefish.Tests
{
    [TestFixture]
    public class SetupTests
    {
        [Test]
        public void CanSetupBasicFramework()
        {
            const string namespaceRoot = "Test";

            Core.Reset();
            Core.Configure()
                .WithDomainNamespaceRoot(namespaceRoot)
                .UseInMemoryEventStore()
                .UseInMemoryCache()
                .UseMassTransitAggregateUpdatePublisher(
                    ServiceBusFactory.New(sbc => { sbc.ReceiveFrom("loopback://localhost/queue"); }))
                .Done();

            Assert.That(Core.Instance.NamespaceRoot, Is.EqualTo(namespaceRoot));
            Assert.That(Core.Instance.EventStore, Is.Not.Null);
            Assert.That(Core.Instance.Cache, Is.Not.Null);
            Assert.That(Core.Instance.AggregateUpdatePublisher, Is.Not.Null);
        }
    }
}