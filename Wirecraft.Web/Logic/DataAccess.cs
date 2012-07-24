using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Wirecraft.Web.Common;

namespace Wirecraft.Web.Logic
{
    public class DataAccess
    {
        public Data.SqlDbContext db { get; set; }
        public DataAccess() {
            db = new Data.SqlDbContext();
        }
        public string getDataGraph() {
            var orders = db.orders
                .Select(x => new Models.Order
                {
                    orderID = x.orderID,
                    discount = x.discount,
                    productIDs = db.orders.Where(y => y.orderID == x.orderID).FirstOrDefault().products.Select(y => y.productID),
                    customerID = x.customerID,
                    status = x.status,
                    timeStamp = x.timeStamp,
                    address = x.address
                }).ToArray();


            var customers = db.customers
                .Select(x => new Models.Customer
                {
                    name = x.name,
                    customerID = x.customerID,
                    birthDay = x.birthDay,
                    photoName = x.photoName,
                    timeStamp = x.timeStamp
                }).ToArray();

            var products = db.products
                .Select(x => new Models.Product
                {
                    name = x.name,
                    productID = x.productID,
                    price = x.price,
                    fileIDs = x.files.Select(y => y.blob).Select(y => y.blobID),
                    description = x.description,
                    timeStamp = x.timeStamp
                }).ToArray();

            var blobs = db.blobs
                .Select(x => new Models.Blob
                {
                    name = x.name,
                    blobID = x.blobID,
                    type = x.type,
                    timeStamp = x.timeStamp
                }).ToArray();

            return JsonConvert.SerializeObject(new
            {
                orders = orders,
                customers = customers,
                products = products,
                blobs = blobs
            });

        }

        public Web.Data.Blob getBlobById(int id, BlobType type)
        {
            return db.blobs
                .Where(x => x.blobID == id && x.type == type)
                .FirstOrDefault();
        }

        public Data.Customer getCustomerByID(int id) {
            return db.customers
                .Where(x => x.customerID == id)
                .FirstOrDefault();
        }

        public void updateCustomerPhoto(int id, byte[] photo, string name)
        {
            var customer = db.customers
                .Where(x => x.customerID == id).SingleOrDefault();
            customer.photoData = photo;
            customer.photoName = name;
            customer.timeStamp = DateTime.Now.Date;
            db.SaveChanges();
        }
        public void updateCustomer(Wirecraft.Web.Models.Customer customer) {
            var oldCustomer = db.customers
                .Where(x => x.customerID == customer.customerID)
                .SingleOrDefault();
            oldCustomer.birthDay = customer.birthDay.Date;
            oldCustomer.name = customer.name;
            db.SaveChanges();
        }

    }
}
