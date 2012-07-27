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
        public TraceController() {
        }
        private static object _ui_exp_lock = new object();
        public ActionResult Index()
        {
            return View();
        }



        public string writeTrace(string trace) {
            trace += "/r/n";
            HttpContext.Trace.Warn(trace);
            lock(_ui_exp_lock)
            {
                System.IO.File.AppendAllText(HostingEnvironment.ApplicationPhysicalPath + "/App_Data/ui_exception.txt", trace);
            }
            return "success";
        }

    }
}
