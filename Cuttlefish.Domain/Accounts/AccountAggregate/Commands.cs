using System;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Accounts.AccountAggregate
{
    public class Suspend : ICommand
    {
        public Suspend()
        {
        }

        public Suspend(Guid id, String reason)
        {
            AggregateIdentity = id;
            Reason = reason;
        }

        public String Reason { get; private set; }
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }


    public class Unsuspend : ICommand
    {
        public Unsuspend()
        {
        }

        public Unsuspend(Guid id)
        {
            AggregateIdentity = id;
        }

        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }
}