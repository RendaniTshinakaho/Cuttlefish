using System;
using Cuttlefish.EventStorage.NEventStore;
using Cuttlefish.ExampleApp.Domain.Warehouse;
using NUnit.Framework;

namespace Cuttlefish.ExampleApp.Domain
{
    [TestFixture]
    [Category("Example Application")]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            Core.Reset();
            Core.Instance
                .WithDomainNamespaceRoot("Cuttlefish.ExampleApp.Domain")
                .UseInMemoryEventStore()
                .Done();

            _productName = "Widget X";
            _description = "blah blah blah";
            _itemcode = "X0001";
            _barcode = "123456";
            var newProductCommand = new StartStockingProduct(Guid.NewGuid(), _itemcode, _productName, _description,
                                                             _barcode);
            CommandRouter.ExecuteCommand(newProductCommand);

            _productId = newProductCommand.AggregateIdentity;
        }

        private Guid _productId;
        private string _itemcode;
        private string _productName;
        private string _description;
        private string _barcode;

        [Test]
        public void ProductCanBeDiscontinued()
        {
            CommandRouter.ExecuteCommand(new DiscontinueProduct(_productId));
            var product = AggregateBuilder.Get<ProductAggregate>(_productId);
            Assert.That(product.Discontinued, Is.True);
        }

        [Test]
        public void ProductCanBeRenamed()
        {
            const string newName = "New Name";
            CommandRouter.ExecuteCommand(new Rename(_productId, newName));

            var product = AggregateBuilder.Get<ProductAggregate>(_productId);
            Assert.That(product.Name, Is.EqualTo(newName));
        }

        [Test]
        public void ProductCanBeSuspended()
        {
            CommandRouter.ExecuteCommand(new SuspendSaleOfProduct(_productId));
            var product = AggregateBuilder.Get<ProductAggregate>(_productId);
            Assert.That(product.Suspended, Is.True);
        }

        [Test]
        public void WarehouseCanAcceptMultipleShipments()
        {
            const int quantity = 100;
            const int shipments = 10;
            for (int i = 0; i < shipments; i++)
            {
                CommandRouter.ExecuteCommand(new AcceptShipmentOfProduct(_productId, quantity));
            }
            var product = AggregateBuilder.Get<ProductAggregate>(_productId);
            Assert.That(product.QuantityOnHand, Is.EqualTo(quantity*shipments));
        }

        [Test]
        public void WarehouseCanAcceptShipment()
        {
            const int quantity = 100;
            CommandRouter.ExecuteCommand(new AcceptShipmentOfProduct(_productId, quantity));
            var product = AggregateBuilder.Get<ProductAggregate>(_productId);
            Assert.That(product.QuantityOnHand, Is.EqualTo(quantity));
        }

        [Test]
        public void WarehouseCanBookOutStock()
        {
            const int initialQuantity = 1000;
            const int quantityToBookOut = 800;
            CommandRouter.ExecuteCommand(new AcceptShipmentOfProduct(_productId, initialQuantity));
            CommandRouter.ExecuteCommand(new BookOutStockAgainstOrder(_productId, quantityToBookOut));

            var product = AggregateBuilder.Get<ProductAggregate>(_productId);
            Assert.That(product.QuantityOnHand, Is.EqualTo(initialQuantity - quantityToBookOut));
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
            Assert.That(product.Suspended, Is.False);
            Assert.That(product.Discontinued, Is.False);
        }

        [Test]
        [ExpectedException(typeof (InvalidBarcodeException))]
        public void WarehouseThrowsExceptionWhenInvalidBarcodeIsSelectedForNewProduct()
        {
            var newProductCommand = new StartStockingProduct(Guid.NewGuid(), _itemcode, _productName, _description,
                                                             "invalid barcode");
            CommandRouter.ExecuteCommand(newProductCommand);
        }

        [Test]
        [ExpectedException(typeof (ProductStockingException))]
        public void WarehouseThrowsExceptionWhenNewProductNameIsBlank()
        {
            var newProductCommand = new StartStockingProduct(Guid.NewGuid(), _itemcode, string.Empty, _description,
                                                             _barcode);
            CommandRouter.ExecuteCommand(newProductCommand);
        }

        [Test]
        [ExpectedException(typeof (OutOfStockException))]
        public void WarehouseThrowsOutOfStockExceptionWhenOrderExceedsQuantityOnHand()
        {
            const int initialQuantity = 500;
            const int quantityToBookOut = 800;
            CommandRouter.ExecuteCommand(new AcceptShipmentOfProduct(_productId, initialQuantity));
            CommandRouter.ExecuteCommand(new BookOutStockAgainstOrder(_productId, quantityToBookOut));
        }
    }

    public class OutOfStockException : Exception
    {
    }
}