using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wirecraft.Web.Common;
namespace Wirecraft.Web.Models
{
    public class Order
    {
        public int orderID { get; set; }
        public IEnumerable<int> productIDs { get; set; }
        public double discount { get; set; }
        public int customerID { get; set; }
        public OrderStatus status { get; set; }
        public DateTime timeStamp { get; set; }
        public string address { get; set; }
    }
}