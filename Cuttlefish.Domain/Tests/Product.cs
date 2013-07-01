using System;
using System.Collections.Generic;
using Cuttlefish.Common;
using Cuttlefish.Domain.Catalogue.ProductAggregate;
using Cuttlefish.Domain.Catalogue.ProductsService;
using Cuttlefish.EventStorage.NEventStore;
using NUnit.Framework;

namespace Cuttlefish.Domain.Tests
{
    [TestFixture]
    public class Product : AggregateBase
    {
        [SetUp]
        public void Setup()
        {
            Core.Reset();
            Core.Configure()
                .WithDomainNamespaceRoot("Cuttlefish.Domain")
                .UseInMemoryEventStore()
                .Done();

            _ProductsService = new ProductsService();
            _Cmd = new StockNewProduct("Widget 123", "Some or other description", 100);
            _Id = _Cmd.AggregateIdentity;
            _ProductsService.On(_Cmd);
        }

        public Product(IEnumerable<IEvent> events)
            : base(events)
        {
        }

        public Product()
            : base(new List<IEvent>())
        {
        }

        private ProductsService _ProductsService;
        private Guid _Id;
        private StockNewProduct _Cmd;


        [Test]
        public void AdjustPrice()
        {
            CommandRouter.ExecuteCommand(new AdjustPrice(_Id, 50));
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
        public void StockNewProduct()
        {
            var aggregate = AggregateBuilder.Get<ProductAggregate>(_Id);

            Assert.That(aggregate.Name, Is.EqualTo(_Cmd.Name));
            Assert.That(aggregate.Description, Is.EqualTo(_Cmd.Description));
            Assert.That(aggregate.Id, Is.EqualTo(_Cmd.AggregateIdentity));
            Assert.That(aggregate.Price, Is.EqualTo(_Cmd.Price));
        }
    }
}