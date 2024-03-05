using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase.ClassFiles
{
    public class portfolio
    {
        public string symbol { get; set; }
        public string validity { get; set; }
        public string buysell { get; set; }
        public string holdqty { get; set; }
        public string holdprice { get; set; }
        public string marketprice { get; set; }
        public string exchange { get; set; }
        public string profitloss { get; set; }
    }
}