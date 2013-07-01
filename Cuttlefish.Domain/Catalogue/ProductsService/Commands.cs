using System;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Catalogue.ProductsService
{
    public class StockNewProduct : ICommand
    {
        public StockNewProduct()
        {
        }

        public StockNewProduct(String name, String description, Int32 price)
        {
            AggregateIdentity = Guid.NewGuid();
            Name = name;
            Description = description;
            Price = price;
        }

        public String Name { get; private set; }
        public String Description { get; private set; }
        public Int32 Price { get; private set; }
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }
}