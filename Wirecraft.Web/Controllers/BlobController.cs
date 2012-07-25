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
            if (blob != null)
            {
                var type = MimeTypes.mimes[Path.GetExtension(blob.name).Substring(1)];
                return new FileContentResult(blob.data, type);
            }
            else {
                throw new HttpException(404, "blob Image not found!!");
            }
        }
        public FileContentResult fileByName(string id) {
            DataAccess da = new DataAccess();
            var blob = da.getBlobByName(id);
            if (blob != null)
            {
                var type = MimeTypes.mimes[Path.GetExtension(blob.name).Substring(1)];
                return new FileContentResult(blob.data, type);
            }
            else
            {
                throw new HttpException(404, "blob file not found!!");
            }
        }
        [HttpPost]
		public ActionResult delete(int id) {
			DataAccess da = new DataAccess();
			da.deleteBlob(id);
			return Json(new { success = true });
		}
    }
}
