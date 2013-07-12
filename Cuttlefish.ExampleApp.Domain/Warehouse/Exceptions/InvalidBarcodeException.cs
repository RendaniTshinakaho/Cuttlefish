using System;

namespace Cuttlefish.ExampleApp.Domain.Warehouse
{
    public class InvalidBarcodeException : Exception
    {
        public string Barcode { get; private set; }

        public InvalidBarcodeException(string barcode)
        {
            Barcode = barcode;
        }
    }
}