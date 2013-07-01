using System;
using System.Collections.Generic;
using System.Linq;
using Cuttlefish.Common;
using Cuttlefish.Common.Exceptions;
using Cuttlefish.Domain.Accounts.AccountAggregate;
using Cuttlefish.Domain.Accounts.AccountsService;
using Cuttlefish.EventStorage.NEventStore;
using NUnit.Framework;

namespace Cuttlefish.Tests
{
    [TestFixture]
    public class AggegateBaseTests
    {
        [SetUp]
        public void RunOnce()
        {
            Core.Reset();
            Core.Configure()
                .WithDomainNamespaceRoot("Cuttlefish")
                .UseInMemoryEventStore()
                .Done();
        }

        internal class MyUnhandledCommand : ICommand
        {
            public Guid AggregateIdentity { get; private set; }
            public int Version { get; private set; }
        }

        [Test]
        public void AggregateBuilderWorks()
        {
            Guid id = Guid.NewGuid();
            var cmd1 = new RegisterClient(id, "John", "123", "john@doe.com");
            CommandRouter.ExecuteCommand(cmd1);

            var builtAggregate = AggregateBuilder.Get<AccountAggregate>(id);

            Assert.That(builtAggregate, Is.Not.Null);
            Assert.That(builtAggregate.Email, Is.EqualTo(cmd1.EmailAddres));
            Assert.That(builtAggregate.Name, Is.EqualTo(cmd1.Name));
            Assert.That(builtAggregate.AggregateIdentity, Is.EqualTo(cmd1.AggregateIdentity));
        }

        [Test]
        public void AggregateNameIsCorrect()
        {
            Guid id = Guid.NewGuid();
            var cmd1 = new RegisterClient(id, "John", "123", "john@doe.com");
            CommandRouter.ExecuteCommand(cmd1);
            var builtAggregate = AggregateBuilder.Get<AccountAggregate>(id);
            Assert.That(builtAggregate.TypeName, Is.EqualTo(builtAggregate.GetType().FullName));
        }

        [Test]
        [ExpectedException(typeof (NoHandlerFoundException))]
        public void CommandNotFoundThrowsException()
        {
            Guid id = Guid.NewGuid();
            CommandRouter.ExecuteCommand(new MyUnhandledCommand());
        }

        [Test]
        public void EventsAreAppliedCorrectly()
        {
            Guid id = Guid.NewGuid();
            var e1 = new ClientRegistered(id, "John", "123", "john@doe.com", string.Empty, 5);
            var e2 = new AccountSuspended(id, "Non payment");

            var evts = new List<IEvent> {e1, e2};

            var entity = new AccountAggregate(evts.AsEnumerable());
            Assert.That(entity.Name, Is.EqualTo("John"));
            Assert.That(entity.Suspended, Is.EqualTo(true));
        }
    }
}