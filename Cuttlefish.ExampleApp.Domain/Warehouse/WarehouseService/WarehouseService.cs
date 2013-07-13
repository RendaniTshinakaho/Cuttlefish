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

            new ProductAggregate().FireEvent(new NewProductAddedToWarehouse(cmd.AggregateIdentity, cmd.ItemCode, cmd.Name, cmd.Description, cmd.Barcode));
        }


        public void On(Rename cmd)
        {
            if (!NameIsValid(cmd.Name))
            {
                throw new ProductStockingException(cmd);
            }

            new ProductAggregate().FireEvent(new Renamed(cmd.AggregateIdentity, cmd.Name));
        }

        public void On(AcceptShipmentOfProduct cmd)
        {
            if (!IsValidQuantity(cmd.Quantity))
            {
                throw new InvalidQuantityException();
            }

            new ProductAggregate().FireEvent(new StockReceived(cmd.AggregateIdentity, cmd.Quantity));
        }

        private static bool IsValidQuantity(int quantity)
        {
            return quantity > 0;
        }

        public void On(BookOutStockAgainstOrder cmd)
        {
            throw new NotImplementedException();
        }

        public void On(SuspendSaleOfProduct cmd)
        {
            new ProductAggregate().FireEvent(new Suspended(cmd.AggregateIdentity));
        }

        public void On(DiscontinueProduct cmd)
        {
            new ProductAggregate().FireEvent(new Discontinued(cmd.AggregateIdentity));
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