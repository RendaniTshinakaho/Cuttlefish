using System;
using Cuttlefish.Common;
using Cuttlefish.Domain.Accounts.AccountAggregate;
using Cuttlefish.Domain.Accounts.AccountsService;
using Cuttlefish.EventStorage.NEventStore;
using EventStore;
using EventStore.Serialization;
using NUnit.Framework;

namespace Cuttlefish.Tests
{
    [TestFixture]
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
        public void CanSetupNEventStoreUsingMongo()
        {
            Core.Reset();
            Core.Configure()
                .UseNEventStore(
                    Wireup.Init().UsingMongoPersistence("eventstore", new DocumentObjectSerializer()).Build())
                .Done();

            Assert.That(Core.Instance.GetContainer().Model.HasImplementationsFor<IEventStore>(), Is.True);
        }


        [Test]
        public void CanUseNEventStoreUsingMongo()
        {
            Core.Reset();
            Core.Configure()
                .WithDomainNamespaceRoot("Cuttlefish.Domain")
                .UseNEventStore(
                    Wireup.Init().UsingMongoPersistence("eventstore", new DocumentObjectSerializer()).Build())
                .Done();

            var cmd = new RegisterClient(Guid.NewGuid(), "Test", "test", "test");
            CommandRouter.ExecuteCommand(cmd);
            CommandRouter.ExecuteCommand(new Suspend(cmd.AggregateIdentity, "non payment"));

            var item = AggregateBuilder.Get<AccountAggregate>(cmd.AggregateIdentity);
            Assert.That(item, Is.Not.Null);
            Assert.That(item.AggregateIdentity, Is.EqualTo(cmd.AggregateIdentity));

            Assert.That(Core.Instance.GetContainer().Model.HasImplementationsFor<IEventStore>(), Is.True);
        }
    }
}