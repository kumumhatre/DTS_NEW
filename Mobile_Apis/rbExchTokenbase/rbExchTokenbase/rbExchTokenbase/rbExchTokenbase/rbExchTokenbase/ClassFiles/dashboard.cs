using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rbExchTokenbase.ClassFiles
{
    public class dashboard
    {
        public string symbol { get; set; }
        public double bid { get; set; }
        public Int32 bidqty { get; set; }
        public double ask { get; set; }
        public Int32 askqty { get; set; }
        public double ltp { get; set; }
        public double open { get; set; }
        public double close { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public Int64 vol { get; set; }
        public Int64 oi { get; set; }
        public double change { get; set; }
        public double netchange { get; set; }
        public Int32 lotsize { get; set; }
        public string ltt { get; set; }
        public string lut { get; set; }
        public string expiry { get; set; }
        public string exchange { get; set; }
    }
}