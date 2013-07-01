using System;
using System.Collections.Generic;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Orders.OrdersService
{
    public class OrdersService : AggregateBase
    {
        public OrdersService() : base(new List<IEvent>())
        {
        }

        public OrdersService(IEnumerable<IEvent> events) : base(events)
        {
        }

        public Guid Id { get; private set; }

        public void On(OpenCart cmd)
        {
            throw new NotImplementedException();
        }

        public void When(CartOpened evt)
        {
            throw new NotImplementedException();
        }

        public void When(CartOpenFailed evt)
        {
            throw new NotImplementedException();
        }
    }
}