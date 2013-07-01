using System;

namespace Cuttlefish.Common.Exceptions
{
    public class CoreNotInitialisedException : Exception
    {
        public CoreNotInitialisedException()
            : base("Please call Done() on the core setup.")
        {
        }
    }
}