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
    public class CustomerController : Controller
    {
        //
        // GET: /Customer/

        public ActionResult Index()
        {
            return View();
        }
        public FileContentResult image(int id)
        {
            DataAccess da = new DataAccess();
            var customer = da.getCustomerByID(id);
            if (customer.photoName != null && customer.photoData != null)
            {
                var type = MimeTypes.mimes[Path.GetExtension(customer.photoName).Substring(1)];
                return new FileContentResult(customer.photoData, type);
            }
            else {
                throw new HttpException(404, "Customer Image not found!!");
            }
        }

        [HttpPost]
        public ActionResult uploadPhoto(int id)
        {
            string fileName = Request.QueryString.GetValues("qqfile").FirstOrDefault();
            var fileStream = Request.InputStream;
            using (MemoryStream ms = new MemoryStream())
            {
                fileStream.CopyTo(ms);
                byte[] photo = ms.GetBuffer();
                DataAccess da = new DataAccess();
                da.updateCustomerPhoto(id, photo, fileName);
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult update(Customer customer) {
            DataAccess da = new DataAccess();
            da.updateCustomer(customer);

            return Json(new { success = true });
        }

    }
}
