using System;
using Cuttlefish.Common;
using NUnit.Framework;

namespace Cuttlefish.Caches.BasicInMemory
{
    [TestFixture]
    internal class InMemoryCacheTests
    {
        [SetUp]
        public void Setup()
        {
            Cache = new BasicInMemoryCache();
        }

        private BasicInMemoryCache Cache;

        internal class TestItem1 : IAggregate
        {
            public TestItem1()
            {
                AggregateIdentity = Guid.NewGuid();
                TypeName = GetType().FullName;
            }

            public Guid AggregateIdentity { get; set; }
            public string TypeName { get; private set; }
        }

        internal class TestItem2 : IAggregate
        {
            public TestItem2()
            {
                AggregateIdentity = Guid.NewGuid();
                TypeName = GetType().FullName;
            }

            public Guid AggregateIdentity { get; set; }
            public string TypeName { get; private set; }
        }

        [Test]
        public void CanAddMultipleTypes()
        {
            var testItem1 = new TestItem1();
            Cache.Cache(testItem1);

            var testItem2 = new TestItem2();
            Cache.Cache(testItem2);

            var fetchedItem = Cache.Fetch<TestItem1>(testItem1.AggregateIdentity);
            Assert.That(fetchedItem, Is.Not.Null);

            var expectNullItem = Cache.Fetch<TestItem2>(testItem1.AggregateIdentity);
            Assert.That(expectNullItem, Is.Null);

            var secondItem = Cache.Fetch<TestItem2>(testItem2.AggregateIdentity);
            Assert.That(secondItem, Is.Not.Null);
        }

        [Test]
        public void CanAddToCache()
        {
            var testItem = new TestItem1();
            Cache.Cache(testItem);

            var fetchedItem = Cache.Fetch<TestItem1>(testItem.AggregateIdentity);
            Assert.That(fetchedItem, Is.Not.Null);
            Assert.That(fetchedItem.AggregateIdentity, Is.EqualTo(testItem.AggregateIdentity));
            Assert.That(fetchedItem.TypeName, Is.EqualTo(testItem.TypeName));
        }
    }
}