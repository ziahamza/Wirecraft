using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wirecraft.Web.Models;
using Newtonsoft.Json;
using Wirecraft.Web.Logic;

namespace Wirecraft.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DataAccess dataLayer = new DataAccess();

            ViewBag.Message = "Modify this template to kick-start your ASP.NET MVC application.";
			ViewBag.objGraph = dataLayer.getDataGraph();
            HttpContext.Trace.Write("Hi from trace!!");

            return View();
        }
    }
}
