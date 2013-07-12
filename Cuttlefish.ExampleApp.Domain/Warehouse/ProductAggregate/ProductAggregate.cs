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
        public String ItemCode { get; protected set; }
        public String Name { get; protected set; }
        public String Description { get; protected set; }
        public String Barcode { get; protected set; }
        public Boolean Suspended { get; protected set; }
        public Boolean Discontinued { get; protected set; }
        public Int32 QuantityOnHand { get; protected set; }


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
            throw new NotImplementedException();
        }

        public void When(Discontinued evt)
        {
            throw new NotImplementedException();
        }

        public void When(Suspended evt)
        {
            throw new NotImplementedException();
        }

        public void When(StockReceived evt)
        {
            throw new NotImplementedException();
        }

        public void When(StockBookedOut evt)
        {
            throw new NotImplementedException();
        }
    }
}