namespace Wirecraft.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inita : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Customers", "photo_blobID", "dbo.Blobs");
            DropIndex("dbo.Customers", new[] { "photo_blobID" });
            AddColumn("dbo.Customers", "photoName", c => c.String());
            AddColumn("dbo.Customers", "photoData", c => c.Binary());
            DropColumn("dbo.Customers", "photo_blobID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customers", "photo_blobID", c => c.Int());
            DropColumn("dbo.Customers", "photoData");
            DropColumn("dbo.Customers", "photoName");
            CreateIndex("dbo.Customers", "photo_blobID");
            AddForeignKey("dbo.Customers", "photo_blobID", "dbo.Blobs", "blobID");
        }
    }
}
