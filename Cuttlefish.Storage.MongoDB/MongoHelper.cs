using System;
using System.Configuration;
using MongoDB.Driver;

namespace Cuttlefish.Storage.MongoDB
{
    public static class MongoHelper
    {
        private const string CollectionName = "Aggregates";

        private static MongoClient GetClient()
        {
            if (ConfigurationManager.ConnectionStrings["EventStore"] == null)
            {
                throw new ArgumentNullException("Cannot find connection string named'EventStore'");
            }

            return new MongoClient(ConfigurationManager.ConnectionStrings["EventStore"].ToString());
        }

        public static MongoCollection<T> GetCollection<T>()
        {
            MongoClient client = GetClient();
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase("Test");

            MongoCollection<T> collection = db.GetCollection<T>(CollectionName);
            return collection;
        }

        public static MongoCollection GetCollection()
        {
            MongoClient client = GetClient();
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase("Test");

            MongoCollection collection = db.GetCollection(CollectionName);
            return collection;
        }


        public static void SaveOrUpdateAggregate<T>(T item)
        {
            MongoCollection collection = GetCollection();
            WriteConcernResult result = collection.Insert(item);

            if (result == null)
            {
                throw new Exception("The event store should not be used without write concern of at least 1 enabled.");
            }

            if (!result.Ok)
            {
                throw new CouldNotSaveToDbException(result.ErrorMessage);
            }
        }
    }
}