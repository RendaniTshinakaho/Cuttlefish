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
        private Guid _productId;
        private string _itemcode;
        private string _productName;
        private string _description;
        private string _barcode;

        [SetUp]
        public void Setup()
        {
            Core.Instance
                .WithDomainNamespaceRoot("Cuttlefish.ExampleApp.Domain")
                .UseInMemoryEventStore()
                .Done();

            _productName = "Widget X";
            _description = "blah blah blah";
            _itemcode = "X0001";
            _barcode = "123456";
            var newProductCommand = new StartStockingProduct(Guid.NewGuid(), _itemcode, _productName, _description, _barcode);
            CommandRouter.ExecuteCommand(newProductCommand);

            _productId = newProductCommand.AggregateIdentity;
        }



        [Test]
        [ExpectedException(typeof (InvalidBarcodeException))]
        public void WarehouseThrowsExceptionWhenInvalidBarcodeIsSelectedForNewProduct()
        {
            var newProductCommand = new StartStockingProduct(Guid.NewGuid(), _itemcode, _productName, _description, "invalid barcode");
            CommandRouter.ExecuteCommand(newProductCommand);

        }

        [Test]
        public void WarehouseCanStockNewProduct()
        {
            var product = AggregateBuilder.Get<ProductAggregate>(_productId);
            Assert.That(product, Is.Not.Null);
            Assert.That(product.Barcode, Is.EqualTo(_barcode));
            Assert.That(product.ItemCode, Is.EqualTo(_itemcode));
            Assert.That(product.Name, Is.EqualTo(_productName));
            Assert.That(product.Description, Is.EqualTo(_description));
        }
    }
}
