using System;

namespace Cuttlefish.Common.Exceptions
{
    public class NoHandlerFoundException : Exception
    {
        public NoHandlerFoundException(Type commandType)
        {
            CommandType = commandType;
        }

        public Type CommandType { get; private set; }
    }
}