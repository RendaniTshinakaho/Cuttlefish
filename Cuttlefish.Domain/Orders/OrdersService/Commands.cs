using System;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Orders.OrdersService
{
    public class OpenCart : ICommand
    {
        public OpenCart()
        {
        }

        public OpenCart(Guid id)
        {
            AggregateIdentity = id;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }
}