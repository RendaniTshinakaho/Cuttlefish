using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cuttlefish.Common;
using Fasterflect;
using MongoDB.Bson.Serialization.Attributes;

namespace Cuttlefish
{
    /// <summary>
    ///     Base type which all aggregates inherit from
    /// </summary>
    public abstract class AggregateBase : IAggregate
    {
        /// <summary>
        ///     true when the aggregate is being hydrated in the constructor. used to prevent events from
        ///     being propagated to the message bus every time they are loaded.
        /// </summary>
        private readonly bool _aggregateLoading;


        private string _typeName;

        /// <summary>
        ///     Constructor which allows an aggregate to be hydrated from events.
        /// </summary>
        /// <param name="events">List of events ordered by timestamp</param>
        protected AggregateBase(IEnumerable<IEvent> events)
        {
            _aggregateLoading = true;
            try
            {
                foreach (IEvent @event in events)
                {
                    InvokeEvent(this, @event);
                }
            }
            catch (EventExecutionException ex)
            {
                throw new AggregateRehydrationException(this, ex);
            }
            finally
            {
                _aggregateLoading = false;
            }
        }

        /// <summary>
        ///     The main unique aggregate identifier
        /// </summary>
        [BsonId]
        public Guid AggregateIdentity { get; set; }

        public string TypeName
        {
            get { return _typeName ?? (_typeName = GetType().FullName); }
        }

        protected void FireEvent(IEvent @event)
        {
            EventRouter.FireEventOnAggregate(this, @event);
        }

        private void InvokeEvent(AggregateBase instance, IEvent @event)
        {
            Type eventType = @event.GetType();

            MethodInfo eventHandlerMethod =
                instance.GetType()
                        .Methods()
                        .FirstOrDefault(mi => mi.GetParameters().Any(i => i.ParameterType == eventType));

            if (eventHandlerMethod != null)
            {
                eventHandlerMethod.Invoke(instance, new object[] {@event});

                if (!_aggregateLoading)
                {
                    PublishUpdatedAggregate(instance);
                    UpdateAggregateCache(instance);
                }
            }
        }

        internal void InvokeEvent(IEvent @event)
        {
            InvokeEvent(this, @event);
        }

        /// <summary>
        ///     This updates the aggregate in the cache, if used.
        /// </summary>
        /// <param name="instance"></param>
        private static void UpdateAggregateCache(AggregateBase instance)
        {
            if (Core.Instance.UseCaching && Core.Instance.Cache != null)
            {
                Core.Instance.Cache.Cache(instance);
            }
        }

        /// <summary>
        ///     Publishes the updated version of the aggregate we are working with to a publisher. This could be a queue or a database writer in low volume scenarios.
        /// </summary>
        private static void PublishUpdatedAggregate(AggregateBase instance)
        {
            IAggregateUpdatePublisher publisher = Core.Instance.AggregateUpdatePublisher;
            if (publisher != null)
            {
                Core.Instance.AggregateUpdatePublisher.PublishUpdate(instance);
            }
        }
    }
}