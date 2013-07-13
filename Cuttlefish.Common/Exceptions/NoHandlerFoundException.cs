using System;

namespace Cuttlefish.Common
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