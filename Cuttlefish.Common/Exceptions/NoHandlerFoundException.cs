using System;

namespace Cuttlefish.Common.Exceptions
{
    [Serializable]
    public class NoHandlerFoundException : Exception
    {
        public NoHandlerFoundException(Type commandType)
        {
            CommandType = commandType;
        }

        public Type CommandType { get; private set; }
    }
}