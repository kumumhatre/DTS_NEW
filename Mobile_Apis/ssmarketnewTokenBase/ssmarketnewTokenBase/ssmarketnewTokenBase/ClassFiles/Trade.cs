using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase.ClassFiles
{
    public class Trade
    {
        public string Symbol { get; set; }
        public string Exch { get; set; }
        public string BuySell { get; set; }
        public int Qty { get; set; }
        public double OrdPrice { get; set; }
        public string Orderno { get; set; }
        public string Validity { get; set; }
        public string CreateOn { get; set; }
        public double Price { get; set; }
        public string status { get; set; }
    }
}