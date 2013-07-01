using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Serialization;
using MassTransit;
using MassTransit.SubscriptionConfigurators;
using EventStore.Persistence.DiskStorage;
using NUnit.Framework;

namespace Aitako.Infrastructure.Tests
{
    [TestFixture]
    class MessagingAndEvents
    {
        private IStoreEvents _Store;
        private IServiceBus _MessageBus;

        [SetUp]
        public void Setup()
        {
            _MessageBus = ServiceBusFactory.New(sbc =>
            {
                sbc.UseRabbitMq();
                sbc.ReceiveFrom("rabbitmq://127.0.0.1/DomainEvents");
            });

            _Store = Wireup.Init()
                .LogToOutputWindow()
                .UsingDiskStoragePersistence(@"c:\test")
                .InitializeStorageEngine()
                .UsingSynchronousDispatchScheduler(new DelegateMessageDispatcher((commit) =>
                {
                    foreach (var item in commit.Events)
                    {
                        _MessageBus.Publish(item);
                    }
                }))
               .Build();

        }

        [Test]
        public void SendMessageToRabbit()
        {
            _MessageBus.Publish<AggegateBaseTests.PersonCaptured>(new AggegateBaseTests.PersonCaptured() { Age = 1, Identity = Guid.NewGuid(), Name = "John", Telephone = "123123123" });
        }
    }
}
