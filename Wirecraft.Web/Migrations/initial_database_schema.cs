namespace Wirecraft.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial_database_schema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        productID = c.Int(nullable: false, identity: true),
                        name = c.String(maxLength: 50),
                        price = c.Double(nullable: false),
                        description = c.String(),
                        timeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.productID);
            
            CreateTable(
                "dbo.ProductDocs",
                c => new
                    {
                        blobID = c.Int(nullable: false),
                        productID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.blobID, t.productID })
                .ForeignKey("dbo.Blobs", t => t.blobID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.productID, cascadeDelete: true)
                .Index(t => t.blobID)
                .Index(t => t.productID);
            
            CreateTable(
                "dbo.Blobs",
                c => new
                    {
                        blobID = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 500),
                        data = c.Binary(nullable: false),
                        type = c.Int(nullable: false),
                        timeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.blobID);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        customerID = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50),
                        birthDay = c.DateTime(nullable: false),
                        photoName = c.String(),
                        photoData = c.Binary(),
                        timeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.customerID);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        orderID = c.Int(nullable: false, identity: true),
                        discount = c.Double(nullable: false),
                        customerID = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        orderDate = c.DateTime(nullable: false),
                        timeStamp = c.DateTime(nullable: false),
                        address = c.String(nullable: false, maxLength: 400),
                    })
                .PrimaryKey(t => t.orderID)
                .ForeignKey("dbo.Customers", t => t.customerID, cascadeDelete: true)
                .Index(t => t.customerID);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        productID = c.Int(nullable: false),
                        orderID = c.Int(nullable: false),
                        quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.productID, t.orderID })
                .ForeignKey("dbo.Products", t => t.productID, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.orderID, cascadeDelete: true)
                .Index(t => t.productID)
                .Index(t => t.orderID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.OrderItems", new[] { "orderID" });
            DropIndex("dbo.OrderItems", new[] { "productID" });
            DropIndex("dbo.Orders", new[] { "customerID" });
            DropIndex("dbo.ProductDocs", new[] { "productID" });
            DropIndex("dbo.ProductDocs", new[] { "blobID" });
            DropForeignKey("dbo.OrderItems", "orderID", "dbo.Orders");
            DropForeignKey("dbo.OrderItems", "productID", "dbo.Products");
            DropForeignKey("dbo.Orders", "customerID", "dbo.Customers");
            DropForeignKey("dbo.ProductDocs", "productID", "dbo.Products");
            DropForeignKey("dbo.ProductDocs", "blobID", "dbo.Blobs");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Orders");
            DropTable("dbo.Customers");
            DropTable("dbo.Blobs");
            DropTable("dbo.ProductDocs");
            DropTable("dbo.Products");
        }
    }
}
