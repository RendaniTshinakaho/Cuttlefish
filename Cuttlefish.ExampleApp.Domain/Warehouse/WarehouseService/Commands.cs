using System;
using Cuttlefish.Common;

namespace Cuttlefish.ExampleApp.Domain.Warehouse
{
    public class StartStockingProduct : ICommand
    {
        private readonly int _version;

        private StartStockingProduct()
        {
            _version = 1;
        }

        public StartStockingProduct(Guid aggregateidentity, String itemcode, String name, String description,
                                    String barcode) : this()
        {
            AggregateIdentity = aggregateidentity;
            ItemCode = itemcode;
            Name = name;
            Description = description;
            Barcode = barcode;
        }

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


    public class Rename : ICommand
    {
        private readonly int _version;

        private Rename()
        {
            _version = 1;
        }

        public Rename(Guid aggregateidentity, String name) : this()
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


    public class AcceptShipmentOfProduct : ICommand
    {
        private readonly int _version;

        private AcceptShipmentOfProduct()
        {
            _version = 1;
        }

        public AcceptShipmentOfProduct(Guid aggregateidentity, Int32 quantity) : this()
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


    public class BookOutStockAgainstOrder : ICommand
    {
        private readonly int _version;

        private BookOutStockAgainstOrder()
        {
            _version = 1;
        }

        public BookOutStockAgainstOrder(Guid aggregateidentity, Int32 quantity) : this()
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


    public class SuspendSaleOfProduct : ICommand
    {
        private readonly int _version;

        private SuspendSaleOfProduct()
        {
            _version = 1;
        }

        public SuspendSaleOfProduct(Guid aggregateidentity) : this()
        {
            AggregateIdentity = aggregateidentity;
        }

        public int Version
        {
            get { return _version; }
        }

        public Guid AggregateIdentity { get; private set; }
    }


    public class DiscontinueProduct : ICommand
    {
        private readonly int _version;

        private DiscontinueProduct()
        {
            _version = 1;
        }

        public DiscontinueProduct(Guid aggregateidentity) : this()
        {
            AggregateIdentity = aggregateidentity;
        }

        public int Version
        {
            get { return _version; }
        }

        public Guid AggregateIdentity { get; private set; }
    }
}