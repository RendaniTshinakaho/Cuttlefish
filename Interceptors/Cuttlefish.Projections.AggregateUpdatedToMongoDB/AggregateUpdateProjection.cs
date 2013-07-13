using System;
using Cuttlefish.Common;
using Cuttlefish.Storage.MongoDB;
using MassTransit;

namespace Cuttlefish.Projections.AggregateUpdatedToMongoDB
{
    public class AggregateUpdateProjection : Consumes<IAggregate>.All
    {
        public void Consume(IAggregate message)
        {
            var store = new MongoStorage();
            try
            {
                dynamic x = message;
                store.SaveOrUpdate(x);
                Console.WriteLine("Saved {0} with id: {1}", message.TypeName, message.AggregateIdentity);
            }
            catch (Exception)
            {
                Console.Write("Failed to save {0} with id: {1}", message.TypeName, message.AggregateIdentity);
            }
        }
    }
}