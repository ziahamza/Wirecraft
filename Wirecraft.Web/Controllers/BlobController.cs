using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wirecraft.Web.Logic;

namespace Wirecraft.Web.Controllers
{
    public class BlobController : Controller
    {
        //
        // GET: /Blob/

        public string Index()
        {
            return "Not impleted!!";
        }
        public FileContentResult imageById(int id)
        {
            DataAccess da = new DataAccess();
            var blob = da.getBlobById(id, Common.BlobType.Image);
            var type = "image/" + Path.GetExtension(blob.name).Substring(1);
            return new FileContentResult(blob.data, type);
        }

		public ActionResult delete(int id) {
			DataAccess da = new DataAccess();
			da.deleteBlob(id);
			return Json(new { success = true });
		}
    }
}
