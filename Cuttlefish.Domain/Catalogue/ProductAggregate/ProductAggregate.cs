using System;
using System.Collections.Generic;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Catalogue.ProductAggregate
{
    public class ProductAggregate : AggregateBase
    {
        public ProductAggregate()
            : base(new List<IEvent>())
        {
        }

        public ProductAggregate(IEnumerable<IEvent> events)
            : base(events)
        {
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Price { get; private set; }
        public bool Active { get; private set; }

        public void When(NewProductInCatalogue evt)
        {
            Id = evt.AggregateIdentity;
            Name = evt.Name;
            Description = evt.Description;
            Price = evt.Price;
        }

        public void On(Discontinue cmd)
        {
            var evt = new ProductDiscontinued(cmd.AggregateIdentity, cmd.Reason);
            FireEvent(evt);
        }

        public void On(AdjustPrice cmd)
        {
            var evt = new PriceAdjusted(cmd.AggregateIdentity, cmd.NewPrice, Price);
            FireEvent(evt);
        }

        public void When(ProductDiscontinued evt)
        {
            Active = false;
        }

        public void When(PriceAdjusted evt)
        {
            Price = evt.NewPrice;
        }
    }
}