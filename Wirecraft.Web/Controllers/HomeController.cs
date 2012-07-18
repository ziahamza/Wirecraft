using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wirecraft.Web.Models;

namespace Wirecraft.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SqlDbContext k = new SqlDbContext();
            var j = k.products.AsEnumerable();
            HttpContext.Trace.Write(j.AsQueryable().Count().ToString());
            ViewBag.Message = "Modify this template to kick-start your ASP.NET MVC application.";
            HttpContext.Trace.Write("Hi from trace!!");

            return View();
        }
    }
}
