namespace Wirecraft.Web.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.Migrations.Sql;
    using System.Linq;
    using Wirecraft.Web.Models;


    public class NonSystemTableSqlGenerator : SqlServerMigrationSqlGenerator
    {
        protected override void GenerateMakeSystemTable(
            CreateTableOperation createTableOperation)
        {
        }
    }
    internal sealed class Configuration : DbMigrationsConfiguration<Wirecraft.Web.Models.SqlDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SqlClient", new NonSystemTableSqlGenerator()); 
        }

        protected override void Seed(Wirecraft.Web.Models.SqlDbContext context)
        {
            context.products.AddOrUpdate(
                x => x.name,
                new Product
                {
                    name = "Rasberry PI",
                    description = "Coolest thing ever!!",
                    price = 25,
                    timeStamp = DateTime.Now.Date,
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

            context.customers.AddOrUpdate(
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
            
            context.orders.AddOrUpdate(
                x => x.orderDate,
                new Order
                {
                    products = new List<OrderItem> { 
                       new OrderItem { orderID = 1, productID = 4, quantity = 10},
                       new OrderItem { orderID = 1, productID = 2, quantity = 7},
                       new OrderItem { orderID = 1, productID = 3, quantity = 12}
                    },
                    customerID = context.customers.Where(x => x.name == "Jenny").SingleOrDefault().customerID,
                    discount = 0,
                    orderDate = new DateTime(2012, 01, 12),
                    address = "New Hamilton street, oxford, UK",
                    status = OrderStatus.pending,
                    timeStamp = DateTime.Now.Date
                },
                new Order
                {
                    products = new List<OrderItem> { 
                       new OrderItem { orderID = 2, productID = 1, quantity = 5},
                       new OrderItem { orderID = 2, productID = 2, quantity = 7},
                       new OrderItem { orderID = 2, productID = 3, quantity = 9}
                    },
                    customerID = context.customers.Where(x => x.name == "Kenny").SingleOrDefault().customerID,
                    discount = 0,
                    orderDate = new DateTime(2012, 12, 12),
                    address = "Zixing road, min hang, shanghai, China",
                    status = OrderStatus.pending,
                    timeStamp = DateTime.Now.Date
                },
                new Order
                {
                    products = new List<OrderItem> { 
                       new OrderItem { orderID = 3, productID = 4, quantity = 20},
                       new OrderItem { orderID = 3, productID = 2, quantity = 7},
                       new OrderItem { orderID = 3, productID = 3, quantity = 22}
                    },
                    customerID = context.customers.Where(x => x.name == "Tom").SingleOrDefault().customerID,
                    discount = 0,
                    orderDate = new DateTime(2012, 04, 25),
                    address = "HKUST, Clear Water bay, Kowloon, Hong kong SAR",
                    status = OrderStatus.pending,
                    timeStamp = DateTime.Now.Date
                }
            );
        }
    }
}
