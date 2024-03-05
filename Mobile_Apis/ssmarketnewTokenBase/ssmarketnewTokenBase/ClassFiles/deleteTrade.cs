using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase.ClassFiles
{
    public class deleteTrade
    {
        public string Clientcode { get; set; }
        public string Exchange { get; set; }
        public string Symbol { get; set; }
        public string Buy_Sell { get; set; }
        public int Qty { get; set; }
        public decimal ExecPrice { get; set; }
        public string Timestamp { get; set; }
        public long Orderno { get; set; }
        public string Ipaddress { get; set; }
    }
}