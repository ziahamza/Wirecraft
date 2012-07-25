using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wirecraft.Web.Logic;
using Wirecraft.Web.Models;

namespace Wirecraft.Web.Controllers
{
    public class OrderController : Controller
    {
        //
        // GET: /Order/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult updateProduct(int id, int productID, int quantity)
        {
			DataAccess da = new DataAccess(HttpContext);
            da.updateOrderProduct(id, productID, quantity);

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult update(Order order)
        {
			DataAccess da = new DataAccess(HttpContext);
            da.updateOrder(order);

            return Json(new { success = true });
        }

		[HttpPost]
		public ActionResult add()
		{
			DataAccess da = new DataAccess(HttpContext);
			Order order = da.newOrder();

			return Json(order);
		}

		[HttpPost]
		public ActionResult delete(int id)
		{
			DataAccess da = new DataAccess(HttpContext);
			da.deleteOrder(id);

			return Json(new {success = true});
		}

    }
}
