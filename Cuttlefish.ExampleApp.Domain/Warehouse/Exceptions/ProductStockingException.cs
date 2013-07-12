using System;

namespace Cuttlefish.ExampleApp.Domain.Warehouse
{
    public class ProductStockingException : Exception
    {
        public StartStockingProduct OriginalCommand { get; private set; }

        public ProductStockingException(StartStockingProduct cmd)
        {
            OriginalCommand = cmd;
        }
    }
}