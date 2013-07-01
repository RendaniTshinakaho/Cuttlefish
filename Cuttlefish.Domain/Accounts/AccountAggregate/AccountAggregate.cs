using System.Collections.Generic;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Accounts.AccountAggregate
{
    [DefaultProjection]
    public class AccountAggregate : AggregateBase
    {
        public int Counter = 0;
        public int? SomeValue;

        public AccountAggregate()
            : base(new List<IEvent>())
        {
        }

        public AccountAggregate(IEnumerable<IEvent> events)
            : base(events)
        {
        }

        public string Name { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }

        public bool Suspended { get; private set; }
        public string SuspendReason { get; private set; }

        public void When(ClientRegistered evt)
        {
            Name = evt.Name;
            PhoneNumber = evt.PhoneNumber;
            Email = evt.EmailAddres;
            AggregateIdentity = evt.AggregateIdentity;
            SomeValue = evt.NotNullableField;
        }

        public void On(Suspend cmd)
        {
            var evt = new AccountSuspended(cmd.AggregateIdentity, cmd.Reason);
            FireEvent(evt);
        }

        public void On(Unsuspend cmd)
        {
            var evt = new AccountReinstated(cmd.AggregateIdentity);
            FireEvent(evt);
        }

        public void When(AccountSuspended evt)
        {
            Counter++;
            Suspended = true;
            SuspendReason = evt.Reason;
        }

        public void When(AccountReinstated evt)
        {
            Suspended = false;
            SuspendReason = string.Empty;
        }
    }
}