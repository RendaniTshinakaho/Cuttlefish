using System;
using System.Collections.Generic;

namespace Cuttlefish.Common
{
    public interface IEventStore
    {
        void AddEvent<T>(T evt) where T : class, IEvent;
        IEnumerable<IEvent> GetEvents(Guid streamId);
    }
}