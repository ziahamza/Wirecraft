using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace Wirecraft.Web.Hubs
{
    public class DataAccessHub : Hub {
        public int add(int x, int y) {
            return x + y;
        }
    }
}