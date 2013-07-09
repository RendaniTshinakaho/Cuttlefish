using System;

namespace Cuttlefish.Common.Exceptions
{
    public class CannotResolveInstanceException : Exception
    {
        public CannotResolveInstanceException(string failedToInstantiateInstanceThroughDiContainer, Exception structureMapException):base(failedToInstantiateInstanceThroughDiContainer, structureMapException)
        {
        }
    }
}