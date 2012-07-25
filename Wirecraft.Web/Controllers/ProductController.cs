using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wirecraft.Web.Logic;
using Wirecraft.Web.Models;

namespace Wirecraft.Web.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult uploadFile(int id)
		{
			string fileName = Request.QueryString.GetValues("qqfile").First();
			int type = int.Parse(Request.QueryString.GetValues("type").First());
			fileName = Guid.NewGuid().ToString() + "_" + fileName;
			var fileStream = Request.InputStream;
			using (MemoryStream ms = new MemoryStream())
			{
				fileStream.CopyTo(ms);
				byte[] file = ms.GetBuffer();
				DataAccess da = new DataAccess(HttpContext);
				da.addProductBlob(id, file, type, fileName);
			}

			return Json(new { success = true });
		}
        [HttpPost]
        public ActionResult update(Product product)
        {
			DataAccess da = new DataAccess(HttpContext);
            da.updateProduct(product);

            return Json(new { success = true });
        }

		[HttpPost]
		public ActionResult add()
		{
			DataAccess da = new DataAccess(HttpContext);
			Product product = da.newProduct();

			return Json(product);
		}

		[HttpPost]
		public ActionResult delete(int id)
		{
			DataAccess da = new DataAccess(HttpContext);
			da.deleteProduct(id);

			return Json(new { success = true });
		}
    }
}
