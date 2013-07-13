using System;
using Cuttlefish.Common;

namespace Cuttlefish.ExampleApp.Domain.Warehouse
{
    public class ProductStockingException : Exception
    {
        public ICommand OriginalCommand { get; private set; }

        public ProductStockingException(ICommand cmd)
        {
            OriginalCommand = cmd;
        }
    }
}