using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Misoi.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        public ActionResult NotFound()
        {
            return View("NotFound");
        }

        protected override void HandleUnknownAction(string actionName)
        {
            string errorMsg = "The Action: " + actionName + "\" is not found in controller \"" +
                              this.ControllerContext.RouteData.Values["controller"] + "\"";
            this.View("NotFound").ExecuteResult(this.ControllerContext);
        }
    }
}