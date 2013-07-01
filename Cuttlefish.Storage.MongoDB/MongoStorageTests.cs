using System;
using System.Linq;
using Cuttlefish.Common;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NUnit.Framework;

namespace Cuttlefish.Storage.MongoDB
{
    [TestFixture]
    public class MongoStorageTests
    {
        public class TestAggregate : IAggregate
        {
            public TestAggregate(int findValue)
            {
                FindValue = findValue;
                AggregateIdentity = Guid.NewGuid();
                TypeName = GetType().FullName;
            }

            public int FindValue { get; private set; }

            [BsonId]
            public Guid AggregateIdentity { get; set; }

            public string TypeName { get; private set; }
        }

        [Test]
        public void CanFetchSavedAggregate()
        {
            var storage = new MongoStorage();
            var aggregate = new TestAggregate(1);
            storage.SaveOrUpdate(aggregate);

            var fetchedItem = storage.FetchById<TestAggregate>(aggregate.AggregateIdentity);
            Assert.That(fetchedItem, Is.Not.Null);
        }

        [Test]
        public void CanGetAggregateCollection()
        {
            MongoCollection<dynamic> collection = MongoHelper.GetCollection<dynamic>();
            Assert.That(collection, Is.Not.Null);
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void CanSaveAggregateWithoutErrors(int times)
        {
            var storage = new MongoStorage();
            DateTime startTime = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                storage.SaveOrUpdate(new TestAggregate(i));
            }
            Console.WriteLine(DateTime.Now.Subtract(startTime).TotalMilliseconds);
        }

        [Test]
        public void CanSearchSavedAggregate()
        {
            int valueToFind = 123;
            var storage = new MongoStorage();
            var aggregate = new TestAggregate(valueToFind);
            storage.SaveOrUpdate(aggregate);

            TestAggregate fetchedItem =
                storage.SearchFor<TestAggregate>(i => i.FindValue == valueToFind).FirstOrDefault();
            Assert.That(fetchedItem, Is.Not.Null);
            Assert.That(fetchedItem.FindValue, Is.EqualTo(valueToFind));
        }
    }
}