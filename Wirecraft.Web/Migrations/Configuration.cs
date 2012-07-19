namespace Wirecraft.Web.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.Migrations.Sql;
    using System.Linq;
    using System.Net;
    using Wirecraft.Web.Common;
    using Wirecraft.Web.Data;


    public class NonSystemTableSqlGenerator : SqlServerMigrationSqlGenerator
    {
        protected override void GenerateMakeSystemTable(
            CreateTableOperation createTableOperation)
        {
        }
    }
    internal sealed class Configuration : DbMigrationsConfiguration<SqlDbContext>
    {
        public WebClient wc { get; set; }
        public Configuration()
        {
            wc = new WebClient();
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SqlClient", new NonSystemTableSqlGenerator());
        }

        public void addBlobs(SqlDbContext ctx)
        {
            ctx.blobs.AddOrUpdate(
                x => x.name,
                new Blob
                {
                    type = BlobType.Image,
                    name = "rasberri_pi.jpg",
                    data = wc.DownloadData(@"http://www.geeky-gadgets.com/wp-content/uploads/2012/04/Raspberry-Pi2.jpg"),
                    timeStamp = DateTime.Now.Date
                },
                new Blob
                {
                    type = BlobType.Image,
                    name = "rasberry_pi_real.jpg",
                    data = wc.DownloadData(@"http://programming4.us/image/052012/Linux%20-%20Ninja%20Pi_1.jpg"),
                    timeStamp = DateTime.Now.Date
                },
                new Blob
                {
                    type = BlobType.Image,
                    name = "ipad.jpg",
                    data = wc.DownloadData(@"http://digitalmediacamp.org/wp-content/uploads/2011/12/ipad-2.jpg"),
                    timeStamp = DateTime.Now.Date
                },
                new Blob
                {
                    type = BlobType.Image,
                    name = "ipad_3.png",
                    data = wc.DownloadData(@"http://nichebloggers.org/wp-content/uploads/2011/09/ipad.png"),
                    timeStamp = DateTime.Now.Date
                },
                new Blob
                {
                    type = BlobType.Image,
                    name = "ipod_nano_touch_all.jpg",
                    data = wc.DownloadData(@"http://www.hyratech.com/images/C/apple-ipod-nano-4g.jpg"),
                    timeStamp = DateTime.Now.Date
                },
                new Blob
                {
                    type = BlobType.Image,
                    name = "ipod_nano_touch.jpg",
                    data = wc.DownloadData(@"http://gadgetsin.com/uploads/2010/12/switcheasy_ticker_ipod_nano_6g_watch_band_1.jpg"),
                    timeStamp = DateTime.Now.Date
                },
                new Blob
                {
                    type = BlobType.Image,
                    name = "old_laptop.png",
                    data = wc.DownloadData(@"http://cktechnical.co.uk/images/laptop.png"),
                    timeStamp = DateTime.Now.Date
                },
                new Blob
                {
                    type = BlobType.Image,
                    name = "new_laptop.jpg",
                    data = wc.DownloadData(@"http://www.laptopscanada.ca/ultra-thin-laptop-images/acer-11.6-laptop.jpg"),
                    timeStamp = DateTime.Now.Date
                }

            );

            ctx.SaveChanges();
        }
        public void addProducts(SqlDbContext ctx)
        {
            ctx.products.AddOrUpdate(
                x => x.name,
                new Product
                {
                    name = "Rasberry PI",
                    description = "Coolest thing ever!!",
                    price = 25,
                    timeStamp = DateTime.Now.Date
                },
                new Product
                {
                    name = "Apple IPAD",
                    description = "tablet sort of!",
                    price = 500,
                    timeStamp = DateTime.Now.Date

                },
                new Product
                {
                    name = "Apple ipod nano touch",
                    description = "small touch screen ipod which I use as a watch",
                    price = 125,
                    timeStamp = DateTime.Now.Date
                },
                new Product
                {
                    name = "HP laptop",
                    description = "Good Laptop",
                    price = 500,
                    timeStamp = DateTime.Now.Date
                }
            );

            ctx.SaveChanges();




        }

        public void addProductDocs(SqlDbContext ctx) {
            ctx.productDocs.AddOrUpdate(
                x => new { x.blobID, x.productID },
                new ProductDoc
                {
                    blobID = ctx.blobs.Where(x => x.name == "rasberri_pi.jpg").SingleOrDefault().blobID,
                    productID = ctx.products.Where(x => x.name == "Rasberry PI").SingleOrDefault().productID

                },
                new ProductDoc
                {
                    blobID = ctx.blobs.Where(x => x.name == "rasberry_pi_real.jpg").SingleOrDefault().blobID,
                    productID = ctx.products.Where(x => x.name == "Rasberry PI").SingleOrDefault().productID
                },
                new ProductDoc
                {
                    blobID = ctx.blobs.Where(x => x.name == "ipad.jpg").SingleOrDefault().blobID,
                    productID = ctx.products.Where(x => x.name == "Apple IPAD").SingleOrDefault().productID
                },
                new ProductDoc
                {
                    blobID = ctx.blobs.Where(x => x.name == "ipad_3.png").SingleOrDefault().blobID,
                    productID = ctx.products.Where(x => x.name == "Apple IPAD").SingleOrDefault().productID
                },
                new ProductDoc
                {
                    blobID = ctx.blobs.Where(x => x.name == "ipod_nano_touch_all.jpg").SingleOrDefault().blobID,
                    productID = ctx.products.Where(x => x.name == "Apple ipod nano touch").SingleOrDefault().productID
                },
                new ProductDoc
                {
                    blobID = ctx.blobs.Where(x => x.name == "ipod_nano_touch.jpg").SingleOrDefault().blobID,
                    productID = ctx.products.Where(x => x.name == "Apple ipod nano touch").SingleOrDefault().productID
                },
                new ProductDoc
                {
                    blobID = ctx.blobs.Where(x => x.name == "old_laptop.png").SingleOrDefault().blobID,
                    productID = ctx.products.Where(x => x.name == "HP laptop").SingleOrDefault().productID
                },
                new ProductDoc
                {
                    blobID = ctx.blobs.Where(x => x.name == "new_laptop.jpg").SingleOrDefault().blobID,
                    productID = ctx.products.Where(x => x.name == "HP laptop").SingleOrDefault().productID
                }
            );
        }

        public void addCustomers(SqlDbContext ctx)
        {

            ctx.customers.AddOrUpdate(
                x => x.name,
                new Customer
                {
                    name = "Hamza Zia",
                    birthDay = new DateTime(1993, 12, 04),
                    timeStamp = DateTime.Now.Date
                },
                new Customer
                {
                    name = "Tom",
                    birthDay = new DateTime(1992, 09, 14),
                    timeStamp = DateTime.Now.Date
                },
                new Customer
                {
                    name = "Kenny",
                    birthDay = new DateTime(1992, 07, 14),
                    timeStamp = DateTime.Now.Date
                },
                new Customer
                {
                    name = "Jenny",
                    birthDay = new DateTime(1991, 12, 08),
                    timeStamp = DateTime.Now.Date
                }
            );

            ctx.SaveChanges();
        }

        public void addOrders(SqlDbContext ctx)
        {

            ctx.orders.AddOrUpdate(
                x => x.orderDate,
                new Order
                {
                    products = new List<OrderItem> { 
                       new OrderItem {productID = 4, quantity = 10},
                       new OrderItem {productID = 2, quantity = 7},
                       new OrderItem {productID = 3, quantity = 12}
                    },
                    customerID = ctx.customers.Where(x => x.name == "Jenny").SingleOrDefault().customerID,
                    discount = 0,
                    orderDate = new DateTime(2012, 01, 12),
                    address = "New Hamilton street, oxford, UK",
                    status = OrderStatus.pending,
                    timeStamp = DateTime.Now.Date
                },
                new Order
                {
                    products = new List<OrderItem> { 
                       new OrderItem { productID = 1, quantity = 5},
                       new OrderItem { productID = 2, quantity = 7},
                       new OrderItem { productID = 3, quantity = 9}
                    },
                    customerID = ctx.customers.Where(x => x.name == "Kenny").SingleOrDefault().customerID,
                    discount = 0,
                    orderDate = new DateTime(2012, 12, 12),
                    address = "Zixing road, min hang, shanghai, China",
                    status = OrderStatus.pending,
                    timeStamp = DateTime.Now.Date
                },
                new Order
                {
                    products = new List<OrderItem> { 
                       new OrderItem { productID = 4, quantity = 20},
                       new OrderItem {  productID = 2, quantity = 7},
                       new OrderItem { productID = 3, quantity = 22}
                    },
                    customerID = ctx.customers.Where(x => x.name == "Tom").SingleOrDefault().customerID,
                    discount = 0,
                    orderDate = new DateTime(2012, 04, 25),
                    address = "HKUST, Clear Water bay, Kowloon, Hong kong SAR",
                    status = OrderStatus.pending,
                    timeStamp = DateTime.Now.Date
                }
            );

        }
        
        protected override void Seed(SqlDbContext context)
        {
            this.addBlobs(context);
            this.addProducts(context);
            this.addProductDocs(context);
            this.addCustomers(context);
            this.addOrders(context);
        }
    }
}
