using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wirecraft.Web.Models
{
    public class Product
    {
        public string name { get; set; }
        public int productID { get; set; }
        public double price { get; set; }
        public virtual ICollection<ProductDoc> files { get; set; }
        public string description { get; set; }
        public DateTime timeStamp { get; set; }
    }
    public class ProductDoc {
        public int productID { get; set; }
        public int blobID { get; set; }
    }
}