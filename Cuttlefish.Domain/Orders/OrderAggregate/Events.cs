using System;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Orders.OrderAggregate
{
    public class OrderCancelled : IEvent
    {
        public OrderCancelled()
        {
        }

        public OrderCancelled(Guid id)
        {
            AggregateIdentity = id;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }


    public class OrderCompleted : IEvent
    {
        public OrderCompleted()
        {
        }

        public OrderCompleted(Guid id)
        {
            AggregateIdentity = id;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }
}