using System;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Orders.CartAggregate
{
    public class OrderPlaced : IEvent
    {
        public OrderPlaced()
        {
        }

        public OrderPlaced(Guid id)
        {
            AggregateIdentity = id;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }


    public class ItemAddedToCart : IEvent
    {
        public ItemAddedToCart()
        {
        }

        public ItemAddedToCart(Guid id, Guid productid, Int32 quantity)
        {
            AggregateIdentity = id;
            ProductId = productid;
            Quantity = quantity;
        }

        public Guid ProductId { get; private set; }
        public Int32 Quantity { get; private set; }
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }
}