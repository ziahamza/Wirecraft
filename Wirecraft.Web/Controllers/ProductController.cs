using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wirecraft.Web.Logic;

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
			string fileName = Request.QueryString.GetValues("qqfile").FirstOrDefault();
			int type = int.Parse(Request.QueryString.GetValues("type").FirstOrDefault());
			fileName = Guid.NewGuid().ToString() + "_" + fileName;
			var fileStream = Request.InputStream;
			using (MemoryStream ms = new MemoryStream())
			{
				fileStream.CopyTo(ms);
				byte[] file = ms.GetBuffer();
				DataAccess da = new DataAccess();
				da.addProductBlob(id, file, type, fileName);
			}

			return Json(new { success = true });
		}

    }
}
