using System;
using Cuttlefish.Common;

namespace Cuttlefish.Domain.Accounts.AccountsService
{
    public class RegisterClient : ICommand
    {
        public RegisterClient()
        {
        }

        public RegisterClient(Guid id, String name, String phonenumber, String emailaddres)
        {
            AggregateIdentity = id;
            Name = name;
            PhoneNumber = phonenumber;
            EmailAddres = emailaddres;
        }

        public String Name { get; private set; }
        public String PhoneNumber { get; private set; }
        public String EmailAddres { get; private set; }
        public Guid AggregateIdentity { get; private set; }
        public int Version { get; private set; }
    }
}