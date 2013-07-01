using Cuttlefish.Common;
using Cuttlefish.Domain.Accounts.AccountAggregate;

namespace Cuttlefish.Domain.Accounts.AccountsService
{
    public class AccountsService : IService
    {
        public void On(RegisterClient cmd)
        {
            var evt = new ClientRegistered(cmd.AggregateIdentity, cmd.Name, cmd.PhoneNumber, cmd.EmailAddres, "iojlkij",
                                           5);
            new AccountAggregate.AccountAggregate().FireEvent(evt);
        }
    }
}