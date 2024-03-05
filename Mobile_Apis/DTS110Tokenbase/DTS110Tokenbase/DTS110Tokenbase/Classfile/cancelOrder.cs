using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTS110Tokenbase.Classfile
{
    public class cancelOrder
    {
        public string clientcode { get; set; }
        public int exch { get; set; }
        public double orderno { get; set; }
    }
    public class modifyOrder
    {
        public decimal price { get; set; }
        public int qty { get; set; }
        public double orderno { get; set; }
    }
}