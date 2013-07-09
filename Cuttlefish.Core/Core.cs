using System;
using System.Collections.Generic;
using Cuttlefish.Common;
using Cuttlefish.Common.Exceptions;
using StructureMap;

namespace Cuttlefish
{
    public class Core
    {
        static Core()
        {
            Instance = new Core();
        }

        private Core()
        {
            Container = new Container(container => container.Build());
        }


        public static Core Instance { get; private set; }

        internal IContainer Container { get; set; }
        internal string NamespaceRoot { get; private set; }
        internal bool UseCaching { get; private set; }

        internal IEventStore EventStore
        {
            get
            {
                if (Instance.Container.Model.HasImplementationsFor<IEventStore>())
                    return Instance.Container.GetInstance<IEventStore>();

                return null;
            }
        }

        internal ICache Cache
        {
            get
            {
                if (Instance.Container.Model.HasImplementationsFor<ICache>())
                    return Instance.Container.GetInstance<ICache>();

                return null;
            }
        }

        internal IAggregateUpdatePublisher AggregateUpdatePublisher
        {
            get
            {
                if (Instance.Container.Model.HasImplementationsFor<IAggregateUpdatePublisher>())
                    return Instance.Container.GetInstance<IAggregateUpdatePublisher>();

                return null;
            }
        }

        public IContainer GetContainer()
        {
            return Container;
        }

        /// <summary>
        ///     This method starts the configuration process.
        /// </summary>
        /// <returns></returns>
        public static Core Configure()
        {
            return Instance;
        }

        public Core ConfigureContainer(Action<ConfigurationExpression> config)
        {
            Instance.GetContainer().Configure(config);

            return Instance;
        }

        /// <summary>
        ///     This is used by the reflection code in CommandRouter to establish which assemblies to load and execute domain commands against.
        /// </summary>
        /// <param name="namespaceRoot">This is the beginning part of namespaces containing domain objects, typically a company or project name. Multipart namespaces like Aitako.Project.Domain are also valid.</param>
        /// <returns></returns>
        public Core WithDomainNamespaceRoot(string namespaceRoot)
        {
            Instance.NamespaceRoot = namespaceRoot;
            return Instance;
        }

        internal Core UsingNullEventStore()
        {
            Container.Configure(expression => expression.For<IEventStore>().Use<DummyEventstore>());
            return Instance;
        }

        public Core Done()
        {
            if (GetContainer().Model.HasImplementationsFor<ICache>())
            {
                UseCaching = true;
            }

            if (EventStore == null)
            {
                throw new EventStoreNotDefinedException();
            }

            return Instance;
        }

        internal static void Reset()
        {
            Instance = new Core();
        }

        public static T ResolveInstance<T>()
        {
            try
            {
                return Instance.Container.GetInstance<T>();
            }
            catch (StructureMapException ex)
            {
                throw new Exception("Failed to instantiate instance through DI container.", ex);
            }
        }

        internal class DummyEventstore : IEventStore
        {
            public void AddEvent<T>(T evt) where T : class, IEvent
            {
            }

            public IEnumerable<IEvent> GetEvents(Guid streamId)
            {
                return new List<IEvent>();
            }
        }
    }
}