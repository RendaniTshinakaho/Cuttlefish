using System;
using System.Configuration;
using MassTransit;
using StructureMap;
using Topshelf;

namespace Cuttlefish.Tools.Interceptor
{
    public class ProjectionService : ServiceControl
    {
        private IServiceBus _Bus;

        public bool Start(HostControl hostControl)
        {
            var container = new Container(cfg => cfg.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.AssembliesFromApplicationBaseDirectory();
                    scan.AddAllTypesOf(typeof (IConsumer));
                }));

            try
            {
                _Bus = ServiceBusFactory.New(sbc =>
                    {
                        sbc.UseRabbitMq();
                        sbc.ReceiveFrom(ConfigurationManager.AppSettings["RabbitMQEndPoint"]);
                        sbc.Subscribe(x => x.LoadFrom(container));
                        sbc.Validate();
                    });

                container.Inject(_Bus);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            if (_Bus != null)
            {
                _Bus.Dispose();
            }
            return true;
        }
    }
}