using System;

namespace Cuttlefish.Common
{
    public interface IMessage
    {
        Guid AggregateIdentity { get; }
        int Version { get; }
    }
}