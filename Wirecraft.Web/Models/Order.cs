using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wirecraft.Web.Models
{
    public class Order
    {
        public int orderID { get; set; }
        public double discount { get; set; }
        public ICollection<OrderItem> products { get; set; }
        public Customer customer { get; set; }
        public int customerID { get; set; }
        public OrderStatus status { get; set; }
        public DateTime orderDate { get; set; }
        public DateTime timeStamp { get; set; }
        public String address { get; set; }

    }
    public enum OrderStatus { 
        fulfilled,
        stopped,
        pending
    }
    public class OrderItem {
        public int orderID { get; set; }
        public int productID { get; set; }
        public int quantity { get; set; }
        public Product product { get; set; }
    }
}