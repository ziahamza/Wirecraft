using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Wirecraft.Web.Controllers
{
    public class TraceController : Controller
    {
        //
        // GET: /Trace/

        public ActionResult Index()
        {
            return View();
        }



        public string writeTrace(string trace) {
            trace += "/r/n";
            HttpContext.Trace.Warn(trace);
            System.IO.File.AppendAllText(HostingEnvironment.ApplicationPhysicalPath + "/App_Data/ui_exception.txt", trace);
            return "success";
        }

    }
}
