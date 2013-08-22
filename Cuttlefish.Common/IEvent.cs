using System;

namespace Cuttlefish.Common
{
    public interface IEvent : IMessage
    {
         DateTime Timestamp { get; }
    }
}