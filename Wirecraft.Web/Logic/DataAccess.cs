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
        public string getDataGraph() {
            Data.SqlDbContext db = new Data.SqlDbContext();
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
    }
}
