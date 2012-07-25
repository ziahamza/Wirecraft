using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wirecraft.Web.Common;
namespace Wirecraft.Web.Models
{
    public class Order
    {
		public Order() {
			this.customerID = (new Data.SqlDbContext())
			.customers.First()
			.customerID;
			this.discount = 0;
			this.address = "sample address";
			this.status = OrderStatus.pending;
			this.timeStamp = DateTime.Now.Date;
			this.orderDate = DateTime.Now.Date;
		}
        public int orderID { get; set; }
        public IEnumerable<int> productIDs { get; set; }
        public IEnumerable<int> quantities { get; set; }
        public double discount { get; set; }
        public int customerID { get; set; }
        public OrderStatus status { get; set; }
        public DateTime timeStamp { get; set; }
        public string address { get; set; }
		public DateTime orderDate {get; set;}
    }
}