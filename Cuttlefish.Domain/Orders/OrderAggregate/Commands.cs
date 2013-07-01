using System;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Orders.OrderAggregate
{
    public class Cancel : ICommand
    {
        public Cancel()
        {
        }

        public Cancel(Guid id)
        {
            AggregateIdentity = id;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }


    public class CompleteOrder : ICommand
    {
        public CompleteOrder()
        {
        }

        public CompleteOrder(Guid id)
        {
            AggregateIdentity = id;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }
}