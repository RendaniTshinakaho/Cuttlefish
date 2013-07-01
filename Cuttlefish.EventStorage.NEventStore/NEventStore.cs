using System;
using System.Collections.Generic;
using System.Linq;
using Cuttlefish.Common;
using EventStore;

namespace Cuttlefish.EventStorage.NEventStore
{
    internal class NEventStore : IEventStore
    {
        private readonly IStoreEvents _store;

        public NEventStore(IStoreEvents nEventStore)
        {
            _store = nEventStore;
        }

        public void AddEvent<T>(T evt) where T : class, IEvent
        {
            using (IEventStream stream = _store.OpenStream(evt.AggregateIdentity, 0, int.MaxValue))
            {
                stream.Add(new EventMessage {Body = evt});
                stream.CommitChanges(Guid.NewGuid());
            }
        }

        public IEnumerable<IEvent> GetEvents(Guid streamId)
        {
            using (IEventStream stream = _store.OpenStream(streamId, 0, int.MaxValue))
            {
                IEnumerable<IEvent> events = stream.CommittedEvents.Select(i => i.Body).Cast<IEvent>();
                return events.AsEnumerable();
            }
        }
    }
}