using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wirecraft.Web.Data
{
    public class Customer
    {
        public string name { get; set; }
        public int customerID { get; set; }
        public DateTime birthDay { get; set; }
        public string photoName { get; set; }
        public byte[] photoData { get; set; }
        public ICollection<Order> orders { get; set; }
        public DateTime timeStamp { get; set; }
    }
}