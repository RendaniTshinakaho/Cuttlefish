using System;
using System.Collections.Generic;
using Cuttlefish.Common;

namespace Cuttlefish.ExampleApp.Domain.Warehouse
{
    public class WarehouseService : AggregateBase
    {
        public WarehouseService()
            : base(new List<IEvent>())
        {
        }

        public WarehouseService(IEnumerable<IEvent> events)
            : base(events)
        {
        }

        public void On(StartStockingProduct cmd)
        {
            if (!BarcodeLengthIsCorrect(cmd.Barcode))
            {
                throw new InvalidBarcodeException(cmd.Barcode);
            }

            if (!NameIsValid(cmd.Name))
            {
                throw new ProductStockingException(cmd);
            }

            EventRouter.FireEventOnAggregate<ProductAggregate>(new NewProductAddedToWarehouse(cmd.AggregateIdentity,
                                                                                              cmd.ItemCode, cmd.Name,
                                                                                              cmd.Description,
                                                                                              cmd.Barcode));
        }


        public void On(Rename cmd)
        {
            if (!NameIsValid(cmd.Name))
            {
                throw new ProductStockingException(cmd);
            }

            EventRouter.FireEventOnAggregate<ProductAggregate>(new Renamed(cmd.AggregateIdentity, cmd.Name));
        }

        public void On(AcceptShipmentOfProduct cmd)
        {
            if (!IsValidQuantity(cmd.Quantity))
            {
                throw new InvalidQuantityException();
            }

            EventRouter.FireEventOnAggregate<ProductAggregate>(new StockReceived(cmd.AggregateIdentity, cmd.Quantity));
        }

        private static bool IsValidQuantity(int quantity)
        {
            return quantity > 0;
        }

        public void On(BookOutStockAgainstOrder cmd)
        {
            if (!IsValidQuantity(cmd.Quantity))
            {
                throw new InvalidQuantityException();
            }

            if (!ItemIsInStockForQuantityRequired(cmd.AggregateIdentity, cmd.Quantity))
            {
                throw new OutOfStockException();
            }

            EventRouter.FireEventOnAggregate<ProductAggregate>(new StockBookedOut(cmd.AggregateIdentity, cmd.Quantity));
        }

        private static bool ItemIsInStockForQuantityRequired(Guid aggregateIdentity, int quantityRequired)
        {
            // this could come from a projection to improve performance, but data might be slightly out of date then. 
            // This could cause orders for out of stock items to be processed.
            var product = AggregateBuilder.Get<ProductAggregate>(aggregateIdentity);
            return product.QuantityOnHand >= quantityRequired;
        }

        public void On(SuspendSaleOfProduct cmd)
        {
            EventRouter.FireEventOnAggregate<ProductAggregate>(new Suspended(cmd.AggregateIdentity));
        }

        public void On(DiscontinueProduct cmd)
        {
            EventRouter.FireEventOnAggregate<ProductAggregate>(new Discontinued(cmd.AggregateIdentity));
        }

        #region Rules

        private static bool BarcodeLengthIsCorrect(string barcode)
        {
            const int barcodeLength = 6;
            return barcode.Length == barcodeLength;
        }

        private static bool NameIsValid(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        #endregion
    }
}