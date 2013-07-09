using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cuttlefish.Common;
using Cuttlefish.Common.Exceptions;
using NUnit.Framework;
using Cuttlefish.EventStorage.NEventStore;
using StructureMap;

namespace Cuttlefish.Tests.Core
{
    [TestFixture]
    public class CoreTests
    {
        [Test]
        [ExpectedException(typeof(EventStoreNotDefinedException))]
        public void NoEventstoreDefinedThrowsException()
        {
            Cuttlefish.Core.Reset();
            Cuttlefish.Core.Instance.WithDomainNamespaceRoot("Cuttlefish.Tests.Core").Done();
        }

        [Test]
        public void CanFetchInstance()
        {
            Assert.That(Cuttlefish.Core.Instance, Is.Not.Null);
        }

        [Test]
        public void CanSetupNullEventStoreForTesting()
        {
            Cuttlefish.Core.Reset();
            Cuttlefish.Core.Configure()
                .UsingNullEventStore()
                .WithDomainNamespaceRoot(string.Empty)
                .Done();
            Assert.That(Cuttlefish.Core.Instance.EventStore, Is.InstanceOf<Cuttlefish.Core.DummyEventstore>());
        }

        [Test]
        public void CanSetNamespaceRootThroughFluidAPI()
        {
            const string namespaceString = "Test";
            Cuttlefish.Core.Reset();
            Cuttlefish.Core.Configure()
                .UseInMemoryEventStore()
                .WithDomainNamespaceRoot(namespaceString)
                .Done();

            Assert.That(Cuttlefish.Core.Instance.NamespaceRoot, Is.EqualTo(namespaceString));
        }

        [Test]
        public void CanSetupStructureMapContainer()
        {
            Cuttlefish.Core.Reset();
            Cuttlefish.Core.Instance
                .WithDomainNamespaceRoot("Cuttlefish.Tests.Core")
                .UseInMemoryEventStore()
                .ConfigureContainer(expression => expression.For<ITest>().Use<Test>())
                .Done();

            var instance = Cuttlefish.Core.ResolveInstance<ITest>();
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.TypeOf<Test>());
        }

        [Test]
        public void CacheFoundInContainerSetsIsCachingToTrue()
        {
            Cuttlefish.Core.Reset();
            Cuttlefish.Core.Instance
                .WithDomainNamespaceRoot("Cuttlefish.Tests.Core")
                .UseInMemoryEventStore()
                .ConfigureContainer(expression => expression.For<ICache>().Use<Test>())
                .Done();

            Assert.That(Cuttlefish.Core.Instance.UseCaching, Is.True);
        }

        [Test]
        public void CacheInstanceAvailable()
        {
            Cuttlefish.Core.Reset();
            Cuttlefish.Core.Instance
                .WithDomainNamespaceRoot("Cuttlefish.Tests.Core")
                .UseInMemoryEventStore()
                .ConfigureContainer(expression => expression.For<ICache>().Use<Test>())
                .Done();

            Assert.That(Cuttlefish.Core.Instance.Cache, Is.Not.Null);
            Assert.That(Cuttlefish.Core.Instance.Cache, Is.InstanceOf<ICache>());
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void ResolveInstanceOfUnknownTypeFailsWithException()
        {
            Cuttlefish.Core.Reset();
            Cuttlefish.Core.Instance
                .WithDomainNamespaceRoot("Cuttlefish.Tests.Core")
                .UseInMemoryEventStore()
                .ConfigureContainer(expression => expression.For<ICache>().Use<Test>())
                .Done();

            Cuttlefish.Core.ResolveInstance<string>();
        }

        [Test]
        public void AggregatePublisherInstanceAvailable()
        {
            Cuttlefish.Core.Reset();
            Cuttlefish.Core.Instance
                .WithDomainNamespaceRoot("Cuttlefish.Tests.Core")
                .UseInMemoryEventStore()
                .ConfigureContainer(expression => expression.For<IAggregateUpdatePublisher>().Use<Test>())
                .Done();

            Assert.That(Cuttlefish.Core.Instance.AggregateUpdatePublisher, Is.Not.Null);
            Assert.That(Cuttlefish.Core.Instance.AggregateUpdatePublisher, Is.InstanceOf<IAggregateUpdatePublisher>());
        }
    }

    #region Test Classes
    internal class Test : ITest, ICache, IAggregateUpdatePublisher
    {
        public void Cache<T>(T item) where T : IAggregate
        {
            throw new NotImplementedException();
        }

        public T Fetch<T>(Guid id)
        {
            throw new NotImplementedException();
        }

        public object Fetch(Guid id, Type type)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void PublishUpdate(IAggregate aggregate)
        {
            throw new NotImplementedException();
        }
    }
    internal interface ITest
    {
    }
    #endregion Test Classes
}
