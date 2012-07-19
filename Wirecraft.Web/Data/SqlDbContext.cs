using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Wirecraft.Web.Common;
namespace Wirecraft.Web.Data
{
    [DbModelBuilderVersion(DbModelBuilderVersion.Latest)]
    public class SqlDbContext : DbContext
    {
        public DbSet<Product> products { get; set; }
        public DbSet<Customer> customers { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<Blob> blobs { get; set; }

        public DbSet<ProductDoc> productDocs { get; set; }
        
        protected void buildCustomers(EntityTypeConfiguration<Customer> customerEntity) {
            customerEntity
                .HasKey(x => x.customerID)
                .Property(x => x.name).IsRequired()
                .HasMaxLength(50);

            customerEntity
                .HasMany(x => x.orders)
                .WithRequired(x => x.customer)
                .HasForeignKey(x => x.customerID)
                .WillCascadeOnDelete();

            customerEntity
                .Property(x => x.timeStamp)
                .IsRequired();

            
        }

        protected void buildProducts(EntityTypeConfiguration<Product> productEntity)
        {
            productEntity
                .HasKey(x => x.productID)
                .Property(x => x.name)
                .HasMaxLength(50);

            productEntity
                .Property(x => x.price)
                .IsRequired();

            productEntity
                .Property(x => x.timeStamp)
                .IsRequired();

            productEntity
                .HasMany(x => x.files)
                .WithRequired(x => x.product)
                .HasForeignKey(x => x.productID)
                .WillCascadeOnDelete();
        }
        protected void buildOrders(EntityTypeConfiguration<Order> orderEntity)
        {

            orderEntity
                .HasKey(x => x.orderID)
                .Property(x => x.orderDate)
                .IsRequired();

            orderEntity
                .Property(x => x.status)
                .IsRequired();

            orderEntity
                .Property(x => x.address)
                .IsRequired()
                .HasMaxLength(400);

            orderEntity
                .Property(x => x.timeStamp)
                .IsRequired();

			orderEntity
				.HasRequired(x => x.customer)
				.WithMany(x => x.orders);
        }
        protected void buildOrderItems(EntityTypeConfiguration<OrderItem> orderItemEntity) {
            orderItemEntity
                .HasKey(x => new { x.productID, x.orderID });

            orderItemEntity
                .HasRequired(x => x.order)
                .WithMany(x => x.products)
                .HasForeignKey(x => x.orderID);

            orderItemEntity
                .Property(x => x.quantity)
                .IsRequired();

        }
        protected void buildProductDocs(EntityTypeConfiguration<ProductDoc> productDocEntity) { 
            productDocEntity
                .HasKey(x => new { x.blobID, x.productID });

            productDocEntity
                .HasRequired(x => x.product)
                .WithMany(x => x.files)
                .HasForeignKey(x => x.productID);

            productDocEntity
                .HasRequired(x => x.blob);
        }
        protected void buildBlobs(EntityTypeConfiguration<Blob> blobEntity)
        {
            blobEntity
                .HasKey(x => x.blobID);

            blobEntity
                .Property(x => x.data)
                .IsRequired();

            blobEntity
                .Property(x => x.name)
                .IsRequired()
                .HasMaxLength(500);

            blobEntity
                .Property(x => x.timeStamp)
                .IsRequired();

            blobEntity
                .Property(x => x.type)
                .IsRequired();

            
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            this.buildCustomers(modelBuilder.Entity<Customer>());
            this.buildProducts(modelBuilder.Entity<Product>());
            this.buildProductDocs(modelBuilder.Entity<ProductDoc>());
            this.buildOrders(modelBuilder.Entity<Order>());
            this.buildOrderItems(modelBuilder.Entity<OrderItem>());
            this.buildBlobs(modelBuilder.Entity<Blob>());
            base.OnModelCreating(modelBuilder);
        }
         
    }
}