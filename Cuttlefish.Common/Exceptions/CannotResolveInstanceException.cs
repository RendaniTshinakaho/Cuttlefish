using System;

namespace Cuttlefish.Common
{
    public class CannotResolveInstanceException : Exception
    {
        public CannotResolveInstanceException(string failedToInstantiateInstanceThroughDiContainer,
                                              Exception structureMapException)
            : base(failedToInstantiateInstanceThroughDiContainer, structureMapException)
        {
        }
    }
}