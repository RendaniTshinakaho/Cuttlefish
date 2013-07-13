using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Cuttlefish.Common;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Cuttlefish.Storage.MongoDB
{
    public class MongoStorage : IStorage
    {
        public void SaveOrUpdate<T>(T item) where T : IAggregate
        {
            MongoHelper.SaveOrUpdateAggregate(item);
        }

        public T FetchById<T>(Guid id) where T : IAggregate
        {
            MongoCollection<T> collection = MongoHelper.GetCollection<T>();
            return collection.AsQueryable().FirstOrDefault(i => i.AggregateIdentity == id);
        }

        public IEnumerable<T> SearchFor<T>(Expression<Func<T, bool>> predicate) where T : IAggregate
        {
            MongoCollection<T> collection = MongoHelper.GetCollection<T>();
            return collection.AsQueryable().Where(predicate);
        }
    }
}