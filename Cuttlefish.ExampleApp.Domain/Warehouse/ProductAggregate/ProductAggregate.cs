using System;
using System.Collections.Generic;
using Cuttlefish.Common;

namespace Cuttlefish.ExampleApp.Domain.Warehouse
{
    public class ProductAggregate : AggregateBase
    {
        public ProductAggregate() : base(new List<IEvent>())
        {
        }

        public ProductAggregate(IEnumerable<IEvent> events) : base(events)
        {
        }

        public DateTime LastChanged { get; protected set; }
        public String ItemCode { get; private set; }
        public String Name { get; private set; }
        public String Description { get; private set; }
        public String Barcode { get; private set; }
        public Boolean Suspended { get; private set; }
        public Boolean Discontinued { get; private set; }
        public Int32 QuantityOnHand { get; private set; }

        public void When(NewProductAddedToWarehouse evt)
        {
            AggregateIdentity = evt.AggregateIdentity;
            Barcode = evt.Barcode;
            Name = evt.Name;
            Description = evt.Description;
            Discontinued = false;
            Suspended = false;
            ItemCode = evt.ItemCode;
            LastChanged = evt.Timestamp;
            QuantityOnHand = 0;
        }

        public void When(Renamed evt)
        {
            Name = evt.Name;
        }

        public void When(Discontinued evt)
        {
            Discontinued = true;
        }

        public void When(Suspended evt)
        {
            Suspended = true;
        }

        public void When(StockReceived evt)
        {
            QuantityOnHand += evt.Quantity;
        }

        public void When(StockBookedOut evt)
        {
            QuantityOnHand -= evt.Quantity;
        }
    }
}