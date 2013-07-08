using System;
using System.Collections.Generic;
using System.Threading;
using Cuttlefish.AggregatePublishers.MassTransit;
using Cuttlefish.Common;
using Cuttlefish.Domain.Catalogue.ProductAggregate;
using Cuttlefish.Domain.Catalogue.ProductsService;
using Cuttlefish.EventStorage.NEventStore;
using EventStore;
using EventStore.Serialization;
using MassTransit;
using NUnit.Framework;

namespace Cuttlefish.Domain.Tests
{
    [TestFixture]
    public class Product : AggregateBase
    {
        private ProductsService _ProductsService;
        private Guid _Id;
        private StockNewProduct _Cmd;

        [SetUp]
        public void Setup()
        {


            Core.Reset();
            Core.Configure()
                .WithDomainNamespaceRoot("Cuttlefish.Domain")
                                .UseNEventStore(Wireup.Init().UsingMongoPersistence("eventstore", new DocumentObjectSerializer()).Build())

                 .UseMassTransitAggregateUpdatePublisher(ServiceBusFactory.New(sbc =>
                 {
                     sbc.UseRabbitMq();
                     sbc.ReceiveFrom("rabbitmq://192.168.1.103/AggregateUpdatePublisherTests");
                 }))
                .Done();

            _ProductsService = new ProductsService();
            _Cmd = new StockNewProduct("Widget 123", "Some or other description", 100);
            _Id = _Cmd.AggregateIdentity;
            CommandRouter.ExecuteCommand(_Cmd);
        }

        public Product(IEnumerable<IEvent> events)
            : base(events)
        {
        }

        public Product()
            : base(new List<IEvent>())
        {
        }


        [Test]
        public void AdjustPrice()
        {
            for (int i = 0; i < 1000; i++)
            {
                CommandRouter.ExecuteCommand(new AdjustPrice(_Id, 50));
            }
            var aggregate = AggregateBuilder.Get<ProductAggregate>(_Id);
            Assert.That(aggregate.Price, Is.EqualTo(50));
        }


        [Test]
        public void Discontinue()
        {
            var aggregate = AggregateBuilder.Get<ProductAggregate>(_Id);

            aggregate.On(new Discontinue(_Id, "Blah"));
            Assert.That(aggregate.Active, Is.False);
            aggregate = AggregateBuilder.Get<ProductAggregate>(_Id);
            Assert.That(aggregate.Active, Is.False);
        }

        [Test]
        public void Stock()
        {

            var aggregate = AggregateBuilder.Get<ProductAggregate>(_Id);

            Assert.That(aggregate.Name, Is.EqualTo(_Cmd.Name));
            Assert.That(aggregate.Description, Is.EqualTo(_Cmd.Description));
            Assert.That(aggregate.Id, Is.EqualTo(_Cmd.AggregateIdentity));
            Assert.That(aggregate.Price, Is.EqualTo(_Cmd.Price));
        }
    }
}