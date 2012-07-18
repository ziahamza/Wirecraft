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
            SqlDbContext db = new SqlDbContext();
			var orders = db.orders
				.Where(x => x.status == OrderStatus.pending)
				.ToList();
			orders.ForEach(x => x.customer = db.customers.Where(y => y.customerID == x.customerID).SingleOrDefault());
            ViewBag.Message = "Modify this template to kick-start your ASP.NET MVC application.";
			ViewBag.orders = orders;
            HttpContext.Trace.Write("Hi from trace!!");

            return View();
        }
    }
}
