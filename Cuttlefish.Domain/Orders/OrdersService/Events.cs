using System;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Orders.OrdersService
{
    public class CartOpened : IEvent
    {
        public CartOpened()
        {
        }

        public CartOpened(Guid id)
        {
            AggregateIdentity = id;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }


    public class CartOpenFailed : IEvent
    {
        public CartOpenFailed()
        {
        }

        public CartOpenFailed(Guid id, String reason)
        {
            AggregateIdentity = id;
            Reason = reason;
        }

        public String Reason { get; private set; }
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }
}