using System;
using Cuttlefish.AggregatePublishers.MassTransit;
using Cuttlefish.Domain.Accounts.AccountAggregate;
using Cuttlefish.Domain.Accounts.AccountsService;
using Cuttlefish.EventStorage.NEventStore;
using MassTransit;
using NUnit.Framework;

namespace Cuttlefish.Domain.Tests
{
    [TestFixture]
    public class Account
    {
        [SetUp]
        public void Setup()
        {
            Core.Reset();
            Core.Configure()
                .WithDomainNamespaceRoot("Cuttlefish.Domain")
                .UseInMemoryEventStore()
                .UseMassTransitAggregateUpdatePublisher(ServiceBusFactory.New(sbc =>
                    {
                        sbc.UseRabbitMq();
                        sbc.ReceiveFrom("rabbitmq://127.0.0.1/AggregateUpdatePublisherTests");
                    }))
                .Done();

            _AccountsService = new AccountsService();
            _Id = Guid.NewGuid();
            _Cmd = new RegisterClient(_Id, "XYZ Computers", "011 1231123", "john@doe.com");
            CommandRouter.ExecuteCommand(_Cmd);
        }

        private AccountsService _AccountsService;
        private Guid _Id;
        private RegisterClient _Cmd;

        [Test]
        public void RegisterClient()
        {
            var aggregate = AggregateBuilder.Get<AccountAggregate>(_Id);
            Assert.That(aggregate.Name, Is.EqualTo(_Cmd.Name));
            Assert.That(aggregate.Email, Is.EqualTo(_Cmd.EmailAddres));
            Assert.That(aggregate.AggregateIdentity, Is.EqualTo(_Cmd.AggregateIdentity));
            Assert.That(aggregate.PhoneNumber, Is.EqualTo(_Cmd.PhoneNumber));
        }

        [Test]
        public void ReinstateClient()
        {
            var cmd = new Suspend(_Id, "Non payment of invoice 110");
            CommandRouter.ExecuteCommand(cmd);
            var aggregate = AggregateBuilder.Get<AccountAggregate>(_Id);
            Assert.That(aggregate.Suspended, Is.True);

            var cmd2 = new Unsuspend(_Id);
            CommandRouter.ExecuteCommand(cmd2);
            aggregate = AggregateBuilder.Get<AccountAggregate>(_Id);
            Assert.That(aggregate.Suspended, Is.False);
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void SpeedTestWithCaching(int times)
        {
            DateTime now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                var cmd = new Suspend(_Id, i.ToString());
                CommandRouter.ExecuteCommand(cmd);
            }

            var aggregate = AggregateBuilder.Get<AccountAggregate>(_Id);
            Assert.That(aggregate.Counter, Is.EqualTo(times));

            Console.WriteLine("Took {0} milliseconds to run {1} times.", DateTime.Now.Subtract(now).Milliseconds, times);
        }

        [Test]
        public void SuspendClient()
        {
            var cmd = new Suspend(_Id, "Non payment of invoice 110");
            CommandRouter.ExecuteCommand(cmd);
            var aggregate = AggregateBuilder.Get<AccountAggregate>(_Id);
            Assert.That(aggregate.Suspended, Is.True);
        }
    }
}