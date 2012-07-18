using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wirecraft.Web.Models
{
    public class Blob
    {
        public string name { get; set; }
        public int blobID { get; set; }
        public byte[] data { get; set; }
        public BlobType type { get; set; }
        public DateTime timeStamp { get; set; }
    }

    public enum BlobType { 
        Image,
        Doc,
        Video,
        Other
    }
}