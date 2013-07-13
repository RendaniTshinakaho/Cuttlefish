using System;
using Cuttlefish.Common;
using EventStore;

namespace Cuttlefish.EventStorage.NEventStore
{
    public static class Extenstions
    {
        public static Core UseNEventStore(this Core core, IStoreEvents nEventStore)
        {
            if (core.GetContainer().Model.HasImplementationsFor<IEventStore>())
            {
                throw new EventStoreAlreadyConfiguredException();
            }

            try
            {
                core.GetContainer()
                    .Configure(
                        expression =>
                        expression.For<IEventStore>()
                                  .Singleton()
                                  .Use<NEventStore>()
                                  .Ctor<IStoreEvents>()
                                  .Is(nEventStore));
            }
            catch (Exception)
            {
                throw new Exception("Failed to execute wireup for event store.");
            }

            return core;
        }

        public static Core UseInMemoryEventStore(this Core core)
        {
            if (core.EventStore != null)
            {
                throw new EventStoreAlreadyConfiguredException();
            }

            try
            {
                core.GetContainer()
                    .Configure(
                        expression =>
                        expression.For<IEventStore>()
                                  .Singleton()
                                  .Use<NEventStore>()
                                  .Ctor<IStoreEvents>()
                                  .Is(Wireup.Init().UsingInMemoryPersistence().Build()));
            }
            catch (Exception)
            {
                throw new Exception("Failed to execute wireup for event store.");
            }

            return core;
        }
    }
}