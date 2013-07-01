using System;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Catalogue.ProductAggregate
{
    public class ProductDiscontinued : IEvent
    {
        public ProductDiscontinued()
        {
        }

        public ProductDiscontinued(Guid id, String reason)
        {
            AggregateIdentity = id;
            Reason = reason;
        }

        public String Reason { get; private set; }
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }

    public class NewProductInCatalogue : IEvent
    {
        public NewProductInCatalogue()
        {
        }

        public NewProductInCatalogue(Guid id, String name, String description, Int32 price)
        {
            AggregateIdentity = id;
            Name = name;
            Description = description;
            Price = price;
        }

        public String Name { get; private set; }
        public String Description { get; private set; }
        public Int32 Price { get; private set; }
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }

    public class PriceAdjusted : IEvent
    {
        public PriceAdjusted()
        {
        }

        public PriceAdjusted(Guid id, Int32 newprice, Int32 oldprice)
        {
            AggregateIdentity = id;
            NewPrice = newprice;
            OldPrice = oldprice;
        }

        public Int32 NewPrice { get; private set; }
        public Int32 OldPrice { get; private set; }
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }
}