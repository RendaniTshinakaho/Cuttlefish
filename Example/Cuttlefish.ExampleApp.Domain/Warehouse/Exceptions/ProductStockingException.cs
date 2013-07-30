using System;
using Cuttlefish.Common;

namespace Cuttlefish.ExampleApp.Domain.Warehouse
{
    public class ProductStockingException : Exception, IDomainException
    {
        public ProductStockingException(ICommand cmd)
        {
            OriginalCommand = cmd;
        }

        public ICommand OriginalCommand { get; private set; }
    }

    public interface IDomainException
    {

    }
}