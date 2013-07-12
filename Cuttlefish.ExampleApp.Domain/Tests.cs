using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cuttlefish.Common;
using Cuttlefish.EventStorage.NEventStore;
using Cuttlefish.ExampleApp.Domain.Warehouse;
using NUnit.Framework;

namespace Cuttlefish.ExampleApp.Domain
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            Core.Instance
                .WithDomainNamespaceRoot("Cuttlefish.ExampleApp.Domain")
                .UseInMemoryEventStore()
                .Done();
        }

        [Test]
        public void WarehouseCanStockNewProduct()
        {
            var newProductCommand = new StartStockingProduct(Guid.NewGuid(), "X0001", "Widget X", "blah blah blah", "123456");
            CommandRouter.ExecuteCommand(newProductCommand);

            var product = AggregateBuilder.Get<ProductAggregate>(newProductCommand.AggregateIdentity);
            Assert.That(product, Is.Not.Null);
            Assert.That(product.Barcode, Is.EqualTo(newProductCommand.Barcode));
            Assert.That(product.Description, Is.EqualTo(newProductCommand.Description));
            Assert.That(product.ItemCode, Is.EqualTo(newProductCommand.ItemCode));
            Assert.That(product.Name, Is.EqualTo(newProductCommand.Name));
        }
    }
}
