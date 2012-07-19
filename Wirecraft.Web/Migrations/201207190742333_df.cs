namespace Wirecraft.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class df : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductDocs", "productID", "dbo.Products");
            DropIndex("dbo.ProductDocs", new[] { "productID" });
            RenameColumn(table: "dbo.ProductDocs", name: "Product_productID", newName: "productID");
            AlterColumn("dbo.ProductDocs", "productID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductDocs", "productID", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.ProductDocs", name: "productID", newName: "Product_productID");
            CreateIndex("dbo.ProductDocs", "productID");
            AddForeignKey("dbo.ProductDocs", "productID", "dbo.Products", "productID");
        }
    }
}
