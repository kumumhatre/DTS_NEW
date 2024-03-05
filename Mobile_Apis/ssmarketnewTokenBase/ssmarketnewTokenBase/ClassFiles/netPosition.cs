using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase.ClassFiles
{
    public class netPosition
    {
        public string exch { get; set; }
        public string symbol { get; set; }
        public int netqty { get; set; }
        public string stringvalidity { get; set; }
        public int BQty { get; set; }
        public decimal buyprice { get; set; }
        public int SQty { get; set; }
        public decimal sellprice { get; set; }
        public decimal p_l { get; set; }
        public decimal mtm { get; set; }

    }

    public class topPanel
    {
        public decimal netmtm { get; set; }
        public decimal realisedpl { get; set; }
        public decimal unrealisedpl { get; set; }
    }
}