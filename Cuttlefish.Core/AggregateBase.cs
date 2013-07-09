﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cuttlefish.Common;
using Cuttlefish.Common.Exceptions;
using Fasterflect;
using MongoDB.Bson.Serialization.Attributes;

namespace Cuttlefish
{
    public abstract class AggregateBase : IAggregate
    {
        private readonly bool _aggregateLoading;
        private string _typeName;

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

        [BsonId]
        public Guid AggregateIdentity { get; set; }

        public string TypeName
        {
            get { return _typeName ?? (_typeName = GetType().FullName); }
        }

        public void FireEvent(IEvent @event)
        {
            try
            {
                InvokeEvent(this, @event);
                Core.Instance.EventStore.AddEvent(@event);
            }
            catch
            {
                throw new EventExecutionException(this, @event);
            }
        }

        protected void InvokeEvent(AggregateBase instance, IEvent @event)
        {
            Type eventType = @event.GetType();
            // could probably do with some caching here. should be pretty easy to do.
            MethodInfo eventHandlerMethod =
                instance.GetType()
                        .Methods()
                        .FirstOrDefault(mi => mi.GetParameters().Any(i => i.ParameterType == eventType));

            if (eventHandlerMethod != null)
            {
                eventHandlerMethod.Invoke(instance, new object[] { @event });

                if (!_aggregateLoading)
                {
                    PublishUpdatedAggregate(instance);
                    UpdateAggregateCache(instance);
                }
            }
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