using NUnit.Framework;

namespace Cuttlefish.Caches.BasicInMemory
{
    [TestFixture]
    internal class InMemoryCacheExtensionTests
    {
        [Test]
        public void CanSetUpInMemoryCacheUsingExtensionMethod()
        {
            Core.Reset();
            Core.Configure()
                .UseInMemoryCache()
                .UsingNullEventStore()
                .Done();

            Assert.That(Core.Instance.UseCaching, Is.True);
        }
    }
}