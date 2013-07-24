using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cuttlefish.ExampleApp.Domain.Warehouse;

namespace Cuttlefish.ExampleApp.UI.Web.Controllers
{
    public class WarehouseController : Controller
    {
        //
        // GET: /Warehouse/

        public ActionResult Index()
        {
            return View();
        }

        public EmptyResult StartStockingProduct(string itemCode, string name, string description, string barcode)
        {
            CommandRouter.ExecuteCommand(new StartStockingProduct(Guid.NewGuid(), itemCode, name, description, barcode));
            return new EmptyResult();
        }
    }
}
