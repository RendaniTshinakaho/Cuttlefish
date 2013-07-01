using System;
using System.Collections.Generic;
using Cuttlefish.Common;
using Cuttlefish.Domain.Catalogue.ProductAggregate;
using Cuttlefish.Domain.Tests;

namespace Cuttlefish.Domain.Catalogue.ProductsService
{
    public class ProductsService : AggregateBase
    {
        public ProductsService()
            : base(new List<IEvent>())
        {
        }

        public ProductsService(IEnumerable<IEvent> events)
            : base(events)
        {
        }

        public Guid Id { get; private set; }

        public void On(StockNewProduct cmd)
        {
            new Product().FireEvent(new NewProductInCatalogue(cmd.AggregateIdentity, cmd.Name, cmd.Description,
                                                              cmd.Price));
        }
    }
}