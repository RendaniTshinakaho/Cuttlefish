using Cuttlefish.Common;
using Cuttlefish.Common.Exceptions;
using EventStore;
using NUnit.Framework;

namespace Cuttlefish.EventStorage.NEventStore
{
    [TestFixture]
    [Category("Integration")]
    public class NEventstoreSetupTests
    {
        [Test]
        public void CanSetupNEventStore()
        {
            Core.Reset();
            Core.Configure()
                .UseNEventStore(Wireup.Init().UsingInMemoryPersistence().Build())
                .Done();

            Assert.That(Core.Instance.GetContainer().Model.HasImplementationsFor<IEventStore>(), Is.True);
            Assert.That(Core.Instance.EventStore, Is.Not.Null);
        }

        [Test]
        public void CanSetupNEventStoreForInMemoryTesting()
        {
            Core.Reset();
            Core.Configure()
                .UseInMemoryEventStore()
                .Done();

            Assert.That(Core.Instance.GetContainer().Model.HasImplementationsFor<IEventStore>(), Is.True);
        }

        [Test]
        [ExpectedException(typeof (EventStoreAlreadyConfiguredException))]
        public void EventStoreSetupThrowsExceptionWhenAlreadyConfigured()
        {
            Core.Reset();
            Core.Configure()
                .UseInMemoryEventStore()
                .UseInMemoryEventStore()
                .Done();

            Assert.That(Core.Instance.GetContainer().Model.HasImplementationsFor<IEventStore>(), Is.True);
        }
    }
}