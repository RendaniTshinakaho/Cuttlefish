using System;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Orders.CartAggregate
{
    public class ConvertToOrder : ICommand
    {
        public ConvertToOrder()
        {
        }

        public ConvertToOrder(Guid id)
        {
            AggregateIdentity = id;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }


    public class ClearCart : ICommand
    {
        public ClearCart()
        {
        }

        public ClearCart(Guid id)
        {
            AggregateIdentity = id;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }


    public class AddItem : ICommand
    {
        public AddItem()
        {
        }

        public AddItem(Guid id, Guid productid, Int32 quantity)
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


    public class RemoveItem : ICommand
    {
        public RemoveItem()
        {
        }

        public RemoveItem(Guid id, Guid productid)
        {
            AggregateIdentity = id;
            ProductId = productid;
        }

        public Guid ProductId { get; private set; }
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }
}