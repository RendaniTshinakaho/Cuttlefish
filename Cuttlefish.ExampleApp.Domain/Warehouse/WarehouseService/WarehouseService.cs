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
            if (!BarcodeLengthIsCorrect(cmd))
            {
                throw new InvalidBarcodeException(cmd.Barcode);
            }

            new ProductAggregate().FireEvent(new NewProductAddedToWarehouse(cmd.AggregateIdentity, cmd.ItemCode, cmd.Name, cmd.Description, cmd.Barcode));
        }


        public void On(Rename cmd)
        {
            throw new NotImplementedException();
        }

        public void On(AcceptShipmentOfProduct cmd)
        {
            throw new NotImplementedException();
        }

        public void On(BookOutStockAgainstOrder cmd)
        {
            throw new NotImplementedException();
        }

        public void On(SuspendSaleOfProduct cmd)
        {
            throw new NotImplementedException();
        }

        public void On(DiscontinueProduct cmd)
        {
            throw new NotImplementedException();
        }

        #region Rules

        private static bool BarcodeLengthIsCorrect(StartStockingProduct cmd)
        {
            const int barcodeLength = 6;
            return cmd.Barcode.Length == barcodeLength;
        }

        #endregion
    }
}