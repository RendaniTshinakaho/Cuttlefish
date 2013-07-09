using System;

namespace Cuttlefish.Common.Exceptions
{
    public class NoHandlerFoundException : Exception
    {
        public Type CommandType { get; private set; }

        public NoHandlerFoundException(Type commandType)
        {
            CommandType = commandType;
        }
    }
}