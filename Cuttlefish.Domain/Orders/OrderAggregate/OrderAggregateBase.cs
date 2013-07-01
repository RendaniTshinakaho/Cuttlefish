using System;
using System.Collections.Generic;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Orders.OrderAggregate
{
    public class OrderAggregateBase : AggregateBase
    {
        public OrderAggregateBase() : base(new List<IEvent>())
        {
        }

        public OrderAggregateBase(IEnumerable<IEvent> events) : base(events)
        {
        }

        public Guid Id { get; private set; }
        public object Items { get; private set; }
        public Int32 Total { get; private set; }
        public Boolean Cancelled { get; private set; }
        public Boolean Completed { get; private set; }

        public void On(Cancel cmd)
        {
            throw new NotImplementedException();
        }

        public void On(CompleteOrder cmd)
        {
            throw new NotImplementedException();
        }

        public void When(OrderCancelled evt)
        {
            throw new NotImplementedException();
        }

        public void When(OrderCompleted evt)
        {
            throw new NotImplementedException();
        }
    }
}