using System;
using Cuttlefish.Common;

namespace Cuttlefish.ExampleApp.Domain.Warehouse
{
    public class ProductStockingException : Exception
    {
        public ProductStockingException(ICommand cmd)
        {
            OriginalCommand = cmd;
        }

        public ICommand OriginalCommand { get; private set; }
    }
}