using System;
using System.Collections.Generic;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Orders.CartAggregate
{
    public class CartAggregateBase : AggregateBase
    {
        public CartAggregateBase() : base(new List<IEvent>())
        {
        }

        public CartAggregateBase(IEnumerable<IEvent> events) : base(events)
        {
        }

        public Guid Id { get; private set; }
        public Object Items { get; private set; }

        public void On(ConvertToOrder cmd)
        {
            throw new NotImplementedException();
        }

        public void On(ClearCart cmd)
        {
            throw new NotImplementedException();
        }

        public void On(AddItem cmd)
        {
            throw new NotImplementedException();
        }

        public void On(RemoveItem cmd)
        {
            throw new NotImplementedException();
        }

        public void When(OrderPlaced evt)
        {
            throw new NotImplementedException();
        }

        public void When(ItemAddedToCart evt)
        {
            throw new NotImplementedException();
        }
    }
}