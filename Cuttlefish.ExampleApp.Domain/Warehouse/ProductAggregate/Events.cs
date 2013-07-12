using System;
using Cuttlefish.Common;

namespace Cuttlefish.ExampleApp.Domain.Warehouse
{
    public class NewProductAddedToWarehouse : IEvent
    {
        private readonly int _version;

        private NewProductAddedToWarehouse()
        {
            _version = 1;
        }

        public NewProductAddedToWarehouse(Guid aggregateidentity, String itemcode, String name, String description,
                                          String barcode) : this()
        {
            AggregateIdentity = aggregateidentity;
            ItemCode = itemcode;
            Name = name;
            Description = description;
            Barcode = barcode;
            Timestamp = DateTime.Now;
        }

        public DateTime Timestamp { get; private set; }
        public String ItemCode { get; private set; }
        public String Name { get; private set; }
        public String Description { get; private set; }
        public String Barcode { get; private set; }

        public int Version
        {
            get { return _version; }
        }

        public Guid AggregateIdentity { get; private set; }
    }

    public class Renamed : IEvent
    {
        private readonly int _version;

        private Renamed()
        {
            _version = 1;
        }

        public Renamed(Guid aggregateidentity, String name) : this()
        {
            AggregateIdentity = aggregateidentity;
            Name = name;
        }

        public String Name { get; private set; }

        public int Version
        {
            get { return _version; }
        }

        public Guid AggregateIdentity { get; private set; }
    }

    public class Discontinued : IEvent
    {
        private readonly int _version;

        private Discontinued()
        {
            _version = 1;
        }

        public Discontinued(Guid aggregateidentity) : this()
        {
            AggregateIdentity = aggregateidentity;
        }

        public int Version
        {
            get { return _version; }
        }

        public Guid AggregateIdentity { get; private set; }
    }

    public class Suspended : IEvent
    {
        private readonly int _version;

        private Suspended()
        {
            _version = 1;
        }

        public Suspended(Guid aggregateidentity) : this()
        {
            AggregateIdentity = aggregateidentity;
        }

        public int Version
        {
            get { return _version; }
        }

        public Guid AggregateIdentity { get; private set; }
    }

    public class StockReceived : IEvent
    {
        private readonly int _version;

        private StockReceived()
        {
            _version = 1;
        }

        public StockReceived(Guid aggregateidentity, Int32 quantity) : this()
        {
            AggregateIdentity = aggregateidentity;
            Quantity = quantity;
        }

        public Int32 Quantity { get; private set; }

        public int Version
        {
            get { return _version; }
        }

        public Guid AggregateIdentity { get; private set; }
    }

    public class StockBookedOut : IEvent
    {
        private readonly int _version;

        private StockBookedOut()
        {
            _version = 1;
        }

        public StockBookedOut(Guid aggregateidentity, Int32 quantity, Int32 ordernumber) : this()
        {
            AggregateIdentity = aggregateidentity;
            Quantity = quantity;
            OrderNumber = ordernumber;
        }

        public Int32 Quantity { get; private set; }
        public Int32 OrderNumber { get; private set; }

        public int Version
        {
            get { return _version; }
        }

        public Guid AggregateIdentity { get; private set; }
    }
}