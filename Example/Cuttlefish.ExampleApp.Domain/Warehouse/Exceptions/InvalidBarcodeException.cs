using System;

namespace Cuttlefish.ExampleApp.Domain.Warehouse
{
    public class InvalidBarcodeException : Exception
    {
        public InvalidBarcodeException(string barcode)
        {
            Barcode = barcode;
        }

        public string Barcode { get; private set; }
    }
}