using System;
using System.Collections.Generic;
using Cuttlefish.Common;
using Cuttlefish.Common.Exceptions;
using Cuttlefish.EventStorage.NEventStore;
using NUnit.Framework;

namespace Cuttlefish.Tests.Core
{
    [TestFixture]
    public class AggegateBaseTests
    {
        [SetUp]
        public void RunOnce()
        {
            Cuttlefish.Core.Reset();
            Cuttlefish.Core.Configure()
                      .WithDomainNamespaceRoot("Cuttlefish.Tests")
                      .UseInMemoryEventStore()
                      .Done();
        }

        [Test]
        public void AggregateNameIsCorrect()
        {
            Guid id = Guid.NewGuid();
            var cmd = new AddOne(id);
            CommandRouter.ExecuteCommand(cmd);
            var builtAggregate = AggregateBuilder.Get<TestAggregate>(id);
            Assert.That(builtAggregate.TypeName, Is.EqualTo(builtAggregate.GetType().FullName));
        }

        [Test]
        public void CanBuildUpAggregateAfterCommandFired()
        {
            Guid id = Guid.NewGuid();
            var cmd = new AddOne(id);
            CommandRouter.ExecuteCommand(cmd);

            var builtAggregate = AggregateBuilder.Get<TestAggregate>(cmd.AggregateIdentity);

            Assert.That(builtAggregate, Is.Not.Null);
            Assert.That(builtAggregate.Count, Is.EqualTo(1));
        }

        [Test]
        [ExpectedException(typeof (NoHandlerFoundException))]
        public void CommandNotFoundThrowsException()
        {
            CommandRouter.ExecuteCommand(new MyUnhandledCommand());
        }

        [Test]
        public void MultipleCommandsExecuteCorrectly()
        {
            Guid id = Guid.NewGuid();
            var c1 = new AddOne(id);
            CommandRouter.ExecuteCommand(c1);
            var c2 = new MinusOne(id);
            CommandRouter.ExecuteCommand(c2);

            var aggregate = AggregateBuilder.Get<TestAggregate>(id);
            Assert.That(aggregate.Count, Is.EqualTo(0));
        }
    }

    public class MyUnhandledCommand : ICommand
    {
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }

    public class TestAggregate : AggregateBase
    {
        public int Count;

        public TestAggregate()
            : base(new List<IEvent>())
        {
        }

        public TestAggregate(IEnumerable<IEvent> events)
            : base(events)
        {
        }

        public void On(AddOne cmd)
        {
            FireEvent(new OneAdded(cmd.AggregateIdentity));
        }

        public void On(MinusOne cmd)
        {
            FireEvent(new OneSubtracted(cmd.AggregateIdentity));
        }

        public void When(OneAdded evt)
        {
            Count++;
        }

        public void When(OneSubtracted evt)
        {
            Count--;
        }
    }

    public class AddOne : ICommand
    {
        public AddOne(Guid aggregateIdentity)
        {
            AggregateIdentity = aggregateIdentity;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }

    public class MinusOne : ICommand
    {
        public MinusOne(Guid aggregateIdentity)
        {
            AggregateIdentity = aggregateIdentity;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }

    public class OneAdded : IEvent
    {
        public OneAdded(Guid aggregateIdentity)
        {
            AggregateIdentity = aggregateIdentity;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }

    public class OneSubtracted : IEvent
    {
        public OneSubtracted(Guid aggregateIdentity)
        {
            AggregateIdentity = aggregateIdentity;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }
}