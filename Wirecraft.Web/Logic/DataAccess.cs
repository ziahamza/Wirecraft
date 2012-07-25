using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using SignalR;
using SignalR.Hubs;
using Wirecraft.Web.Common;
using Wirecraft.Web.Hubs;

namespace Wirecraft.Web.Logic
{
    public class DataAccess
    {
        public Data.SqlDbContext db { get; set; }
        public DataAccess()
        {
            db = new Data.SqlDbContext();
        }
        public string getDataGraph()
        {
            var orders = db.orders
                .Select(x => new Models.Order
                {
                    orderID = x.orderID,
                    discount = x.discount,
                    productIDs = x.products.Select(y => y.productID),
                    quantities = x.products.Select(y => y.quantity),
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
                    fileIDs = x.files.Select(y => y.blobID),
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

        public Data.Customer getCustomerByID(int id)
        {
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

            var ctx = GlobalHost.ConnectionManager.GetHubContext<DataAccessHub>();
            ctx.Clients.updateModel(new[]{
				new {
					op = "update",
					data = new {
						products = new Models.Product[0],
						orders = new Models.Order[0],
						blobs = new Models.Blob[0],
						customers = new Models.Customer[] {
							new Models.Customer {
								name = customer.name,
								customerID = customer.customerID,
								birthDay = customer.birthDay,
								photoName = customer.photoName,
								timeStamp = customer.timeStamp
							}
						}
					}
				}
			});
        }
        public void updateCustomer(Wirecraft.Web.Models.Customer customer)
        {
            customer.timeStamp = DateTime.Now.Date;
            var oldCustomer = db.customers
                .Where(x => x.customerID == customer.customerID)
                .SingleOrDefault();
            oldCustomer.birthDay = customer.birthDay.Date;
            oldCustomer.name = customer.name;
            oldCustomer.timeStamp = customer.timeStamp;
            db.SaveChanges();

            var ctx = GlobalHost.ConnectionManager.GetHubContext<DataAccessHub>();
            ctx.Clients.updateModel(new[]{
				new {
					op = "update",
					data = new {
						products = new Models.Product[0],
						orders = new Models.Order[0],
						blobs = new Models.Blob[0],
						customers = new Models.Customer[] {
							new Models.Customer {
								name = oldCustomer.name,
								customerID = oldCustomer.customerID,
								birthDay = oldCustomer.birthDay,
								photoName = oldCustomer.photoName,
								timeStamp = oldCustomer.timeStamp
							}
						}
					}
				}
			});
        }


        public void addProductBlob(int id, byte[] file, int type, string fileName)
        {
            var product = db.products.Where(x => x.productID == id).SingleOrDefault();

            db.blobs.Add(new Data.Blob
            {
                data = file,
                name = fileName,
                timeStamp = DateTime.Now.Date,
                type = (BlobType)type
            });
            product.timeStamp = DateTime.Now.Date;
            db.SaveChanges();
            db.productDocs.Add(new Data.ProductDoc
            {
                blobID = db.blobs.Where(x => x.name == fileName).SingleOrDefault().blobID,
                productID = product.productID
            });

            db.SaveChanges();
            var ctx = GlobalHost.ConnectionManager.GetHubContext<DataAccessHub>();
            ctx.Clients.updateModel(new dynamic[]{
				new {
						op = "add",
						data = new {
							customers = new Models.Customer[0],
							orders = new Models.Order[0],
							products = new Models.Product[0],
							blobs = db.blobs
								.Where(x => x.name == fileName)
								.Select(x => new Models.Blob
								{
									name = x.name,
									blobID = x.blobID,
									type = x.type,
									timeStamp = x.timeStamp
								}).ToArray()
						}
				},
				new {
					op = "update",
					data = new {
						customers = new Models.Customer[0],
						orders = new Models.Order[0],
						blobs = new Models.Blob[0],
						products = db.products
							.Where(x => x.productID == id)
							.Select(x => new Models.Product
							{
								name = x.name,
								productID = x.productID,
								price = x.price,
								fileIDs = x.files.Select(y => y.blob).Select(y => y.blobID),
								description = x.description,
								timeStamp = x.timeStamp
							}).ToArray()
					}
				}
			});
        }

        public void deleteBlob(int id)
        {
            var blob = db.blobs.Where(x => x.blobID == id).SingleOrDefault();
            var productDocs = db.productDocs.Where(x => x.blobID == id).ToList();
            var productIDs = productDocs.Select(y => y.productID).ToArray();
            var products = db.products.Where(x => productIDs.Contains(x.productID)).ToList();

            db.blobs.Remove(blob);
            productDocs.ForEach(x => db.productDocs.Remove(x));

            db.SaveChanges();


            var diff = new dynamic[] {
				new {
					op = "update",
					data = new {
						customers = new Models.Customer[0],
						orders = new Models.Order[0],
						blobs = new Models.Blob[0],
						products = products
							.Select(x => new Models.Product
							{
								name = x.name,
								productID = x.productID,
								price = x.price,
								fileIDs = x.files.Select(y => y.blobID),
								description = x.description,
								timeStamp = x.timeStamp
							}).ToArray()
					}
				},
				new {
					op = "delete",
					data = new {
						customers = new Models.Customer[0],
						orders = new Models.Order[0],
						products = new Models.Product[0],
						blobs = new Models.Blob[] {
							new Models.Blob {
								name = blob.name,
								blobID = blob.blobID,
								type = blob.type,
								timeStamp = blob.timeStamp
							}
						}
					}
				}
			};


            var ctx = GlobalHost.ConnectionManager.GetHubContext<DataAccessHub>();
            ctx.Clients.updateModel(diff);

        }

        public Data.Blob getBlobByName(string name)
        {
            return db.blobs
                .Where(x => x.name == name)
                .FirstOrDefault();
        }

        public void updateProduct(Models.Product product)
        {
            product.timeStamp = DateTime.Now.Date;
            var oldProduct = db.products
                .Where(x => x.productID == product.productID)
                .SingleOrDefault();
            oldProduct.description = product.description;
            oldProduct.price = product.price;
            oldProduct.name = product.name;
            oldProduct.timeStamp = product.timeStamp;
            db.SaveChanges();

            var ctx = GlobalHost.ConnectionManager.GetHubContext<DataAccessHub>();
            ctx.Clients.updateModel(new[]{
				new {
					op = "update",
					data = new {
						customers = new Models.Customer[0],
						orders = new Models.Order[0],
						blobs = new Models.Blob[0],
						products = new Models.Product[] {
							new Models.Product {
								name = oldProduct.name,
								productID = oldProduct.productID,
								description = oldProduct.description,
								price = oldProduct.price,
								timeStamp = oldProduct.timeStamp,
                                fileIDs = oldProduct.files.Select(y => y.blobID)
							}
						}
					}
				}
			});
        }

        public void updateOrderProduct(int id, int productID, int quantity)
        {
            var ctx = GlobalHost.ConnectionManager.GetHubContext<DataAccessHub>();

            var orderItems = db.orderItems.Where(x => x.orderID == id && x.productID == productID);

            if (quantity == 0) {
                if (orderItems.Count() > 0) 
                    db.orderItems.Remove(orderItems.Single());
            }
            else if (orderItems.Count() > 0)
            {
                var item = orderItems.Single();
                item.quantity = quantity;
                db.SaveChanges();
            }
            else
            {
                db.orderItems
                .Add(new Data.OrderItem {
                    orderID = id,
                    productID = productID,
                    quantity = quantity
                });
            }
            db.SaveChanges();
            ctx.Clients.updateModel(new[]{
				new {
					op = "update",
					data = new {
						customers = new Models.Customer[0],
						products = new Models.Product[0],
						blobs = new Models.Blob[0],
						orders = db.orders.Where(x => x.orderID == id)
                        .Select(order => new Models.Order {
							orderID = order.orderID,
                            discount = order.discount,
                            productIDs = order.products.Select(y => y.productID),
                            quantities = order.products.Select(y => y.quantity),
                            customerID = order.customerID,
                            status = order.status,
                            timeStamp = order.timeStamp,
                            address = order.address
						})
						
					}
				}
			});
        }



        public void updateOrder(Models.Order order)
        {
            var oldOrder = db.orders.Where(x => x.orderID == order.orderID).Single();
            oldOrder.address = order.address;
            oldOrder.discount = order.discount;
            oldOrder.customerID = order.customerID;
            oldOrder.status = order.status;
            oldOrder.timeStamp = DateTime.Now.Date;
            db.SaveChanges();
            var ctx = GlobalHost.ConnectionManager.GetHubContext<DataAccessHub>();
            ctx.Clients.updateModel(new[]{
				new {
					op = "update",
					data = new {
						customers = new Models.Customer[0],
						products = new Models.Product[0],
						blobs = new Models.Blob[0],
						orders = db.orders.Where(x => x.orderID == oldOrder.orderID)
                        .Select(x => new Models.Order {
							orderID = x.orderID,
                            discount = x.discount,
                            productIDs = x.products.Select(y => y.productID),
                            quantities = x.products.Select(y => y.quantity),
                            customerID = x.customerID,
                            status = x.status,
                            timeStamp = x.timeStamp,
                            address = x.address
						})
						
					}
				}
			});
        }

		public Models.Order newOrder()
		{
			Models.Order order = new Models.Order();

			Data.Order dOrder = new Data.Order{
				customerID = order.customerID,
				discount = order.discount,
				address = order.address,
				status = order.status,
				timeStamp = order.timeStamp,
				orderDate = order.orderDate
			};

			db.orders.Add(dOrder);
			db.SaveChanges();
		
			order.orderID = dOrder.orderID;

			var ctx = GlobalHost.ConnectionManager.GetHubContext<DataAccessHub>();
			ctx.Clients.updateModel(new[]{
				new {
					op = "add",
					data = new {
						customers = new Models.Customer[0],
						products = new Models.Product[0],
						blobs = new Models.Blob[0],
						orders = new [] {
							order
						}
						
					}
				}
			});
			return order;
		}

		public void deleteOrder(int orderID)
		{
			var order = db.orders.Where(x => x.orderID == orderID).Single();
			var ctx = GlobalHost.ConnectionManager.GetHubContext<DataAccessHub>();
			ctx.Clients.updateModel(new[]{
				new {
					op = "delete",
					data = new {
						customers = new Models.Customer[0],
						products = new Models.Product[0],
						blobs = new Models.Blob[0],
						orders = new Models.Order[] {
							new Models.Order {
								orderID = orderID
							}
						}
						
					}
				}
			});

			db.orders.Remove(order);
			db.SaveChanges();
		}

		public Models.Customer newCustomer()
		{
			Models.Customer customer = new Models.Customer();
			var dCustomer = new Data.Customer {
				birthDay = customer.birthDay,
				name = customer.name,
				photoName = customer.photoName,
				timeStamp = customer.timeStamp
			};

			db.customers.Add(dCustomer);
			db.SaveChanges();

			customer.customerID = dCustomer.customerID;
			var ctx = GlobalHost.ConnectionManager.GetHubContext<DataAccessHub>();
			ctx.Clients.updateModel(new[]{
				new {
					op = "add",
					data = new {
						orders = new Models.Order[0],
						products = new Models.Product[0],
						blobs = new Models.Blob[0],
						customers = new Models.Customer[] {
							customer
						}
						
					}
				}
			});
			return customer;
		}

		public void deleteCustomer(int id)
		{
			var customer = db.customers.Where(x => x.customerID == id)
				.Single();

			db.customers.Remove(customer);

			var ctx = GlobalHost.ConnectionManager.GetHubContext<DataAccessHub>();
			ctx.Clients.updateModel(new[]{
				new {
					op = "delete",
					data = new {
						orders = new Models.Order[0],
						products = new Models.Product[0],
						blobs = new Models.Blob[0],
						customers = new Models.Customer[] {
							new Models.Customer{
								customerID = customer.customerID,
								birthDay = customer.birthDay,
								name = customer.name,
								timeStamp = customer.timeStamp,
								photoName = customer.photoName
							}
						}
						
					}
				}
			});

			db.SaveChanges();
		}

		public Models.Product newProduct()
		{
			Models.Product product = new Models.Product();
			var newProd = new Data.Product {
				name = product.name,
				description = product.description,
				price = product.price,
				timeStamp = product.timeStamp
			};

			db.products.Add(newProd);
			db.SaveChanges();
			product.productID = newProd.productID;
			var ctx = GlobalHost.ConnectionManager.GetHubContext<DataAccessHub>();
			ctx.Clients.updateModel(new[]{
				new {
					op = "add",
					data = new {
						orders = new Models.Order[0],
						customers = new Models.Customer[0],
						blobs = new Models.Blob[0],
						products = new Models.Product[] {
							product
						}
						
					}
				}
			});

			return product;
		}

		public void deleteProduct(int id)
		{
			var dProduct = db.products.Where(x => x.productID == id)
				.Single();
			db.products.Remove(dProduct);
			var ctx = GlobalHost.ConnectionManager.GetHubContext<DataAccessHub>();
			ctx.Clients.updateModel(new[]{
				new {
					op = "delete",
					data = new {
						orders = new Models.Order[0],
						customers = new Models.Customer[0],
						blobs = new Models.Blob[0],
						products = new Models.Product[] {
							new Models.Product {
								description = dProduct.description,
								name = dProduct.name,
								price = dProduct.price,
								productID = dProduct.productID,
								timeStamp = dProduct.timeStamp
							}
						}
						
					}
				}
			});
			db.SaveChanges();
		}
	}   
}
