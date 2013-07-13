using System;

namespace Cuttlefish.ExampleApp.Domain.Warehouse
{
    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException()
            : base("Quantities for orders placed must exceed 0 and cannot be negative values")
        {

        }
    }
}