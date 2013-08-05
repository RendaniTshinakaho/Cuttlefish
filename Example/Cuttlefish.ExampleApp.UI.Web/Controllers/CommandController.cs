using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cuttlefish.ExampleApp.Domain.Warehouse;

namespace Cuttlefish.ExampleApp.UI.Web.Controllers
{
    public class CommandController : Controller
    {
        public ActionResult Index()
        {
            CommandRouter.ExecuteCommand(new AcceptShipmentOfProduct(Guid.NewGuid(), 123));
            var commands = CommandRouter.GetCommandList();
            return View(commands);
        }

        public ActionResult Exec(string commandName)
        {
            var commands = CommandRouter.GetCommandList();

            var command = commands.FirstOrDefault(i => i.Key == commandName).Value;
            return View(command);
        }
    }
}
