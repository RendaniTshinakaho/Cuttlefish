using System;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Catalogue.ProductAggregate
{
    public class Discontinue : ICommand
    {
        public Discontinue()
        {
        }

        public Discontinue(Guid id, String reason)
        {
            AggregateIdentity = id;
            Reason = reason;
        }

        public String Reason { get; private set; }
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }


    public class AdjustPrice : ICommand
    {
        public AdjustPrice()
        {
        }

        public AdjustPrice(Guid id, Int32 newprice)
        {
            AggregateIdentity = id;
            NewPrice = newprice;
        }

        public Int32 NewPrice { get; private set; }
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }
}