using Cuttlefish.Common;
using Cuttlefish.Domain.Accounts.AccountAggregate;

namespace Cuttlefish.Domain
{
    public class p1 : IProjection
    {
        public string Name { get; private set; }

        public void When(ClientRegistered evt)
        {
        }
    }

    public class p2 : IProjection
    {
        public string Name { get; private set; }

        public void When(ClientRegistered evt)
        {
        }
    }

    public class p3 : IProjection
    {
        public string Name { get; private set; }

        public void When(ClientRegistered evt)
        {
        }
    }
}