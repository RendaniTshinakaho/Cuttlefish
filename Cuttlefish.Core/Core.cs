using System;
using Cuttlefish.Common;
using MassTransit;
using StructureMap;

namespace Cuttlefish
{
    public class Core : IDisposable
    {
        static Core()
        {
            _Instance = new Core();
        }

        private Core()
        {
            Container = new Container(container => container.Build());
        }

        private static Core _Instance { get; set; }

        public static Core Instance
        {
            get { return _Instance; }
        }

        private IContainer Container { get; set; }
        private IServiceBus ProjectionBus { get; set; }

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

        public void Dispose()
        {
            if (Container != null)
            {
                Container.Dispose();
                Container = null;
            }

            if (ProjectionBus != null)
            {
                ProjectionBus.Dispose();
                ProjectionBus = null;
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
            return _Instance;
        }

        /// <summary>
        ///     This is used by the reflection code in CommandRouter to establish which assemblies to load and execute domain commands against.
        /// </summary>
        /// <param name="namespaceRoot">This is the beginning part of namespaces containing domain objects, typically a company or project name. Multipart namespaces like Aitako.Project.Domain are also valid.</param>
        /// <returns></returns>
        public Core WithDomainNamespaceRoot(string namespaceRoot)
        {
            _Instance.NamespaceRoot = namespaceRoot;
            return _Instance;
        }

        public Core Done()
        {
            if (GetContainer().Model.HasImplementationsFor<ICache>())
            {
                UseCaching = true;
            }

            if (EventStore == null)
            {
                throw new Exception(
                    "No event store has been defined. Please make sure that you define one before calling 'Done'");
            }

            return _Instance;
        }

        internal static void Reset()
        {
            _Instance = new Core();
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
    }
}