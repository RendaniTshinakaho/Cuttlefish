using Topshelf;

namespace Cuttlefish.Services.Interceptor
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            return (int) HostFactory.Run(x =>
                {
                    x.Service<EventInterceptionService>(s =>
                        {
                            s.ConstructUsing(name => new EventInterceptionService());
                            s.WhenStarted((helper, control) => helper.Start(control));
                            s.WhenStopped((helper, control) => helper.Start(control));
                        });
                    x.RunAsLocalSystem();
                    x.StartAutomatically();
                    x.SetDescription("Aitako CQRS Projection Service");
                    x.SetDisplayName("Aitako CQRS Projection Service");
                    x.SetServiceName("AitakoCQRSProjectionService");
                });
        }
    }
}