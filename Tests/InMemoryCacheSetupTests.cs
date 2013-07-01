using Cuttlefish.Caches.BasicInMemory;
using Cuttlefish.Common.Exceptions;
using Cuttlefish.EventStorage.NEventStore;
using NUnit.Framework;

namespace Cuttlefish.Tests
{
    [TestFixture]
    public class InMemoryCacheSetupTests
    {
        [Test]
        public void CanSetUpInMemoryCacheUsingExtensionMethod()
        {
            Core.Reset();
            Core.Configure()
                .UseInMemoryEventStore()
                .UseInMemoryCache()
                .Done();

            Assert.That(Core.Instance.UseCaching, Is.True);
        }

        [Test]
        [ExpectedException(typeof (CacheAlreadyConfiguredException))]
        public void ThrowsExceptionWhenTryingToRegisterMultipleCaches()
        {
            Core.Reset();
            Core.Configure()
                .UseInMemoryEventStore()
                .UseInMemoryCache()
                .UseInMemoryCache()
                .Done();
        }
    }
}