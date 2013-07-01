using System;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Accounts.AccountAggregate
{
    public class AccountSuspended : IEvent
    {
        public AccountSuspended()
        {
        }

        public AccountSuspended(Guid id, String reason)
        {
            AggregateIdentity = id;
            Reason = reason;
        }

        public String Reason { get; private set; }
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }

    public class ClientRegistered : IEvent
    {
        public ClientRegistered()
        {
        }

        public ClientRegistered(Guid id, String name, String phonenumber, String emailaddres, string newThing,
                                int? notNullableField)
        {
            NewThing = newThing;
            AggregateIdentity = id;
            Name = name;
            PhoneNumber = phonenumber;
            EmailAddres = emailaddres;
            NewThing = newThing;
            NotNullableField = notNullableField;
        }


        public String Name { get; private set; }
        public String PhoneNumber { get; private set; }
        public String EmailAddres { get; private set; }
        public string NewThing { get; private set; }
        public int? NotNullableField { get; private set; }
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }

    public class AccountReinstated : IEvent
    {
        public AccountReinstated()
        {
        }

        public AccountReinstated(Guid id)
        {
            AggregateIdentity = id;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }
}