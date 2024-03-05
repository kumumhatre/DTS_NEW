using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rbExchTokenbase.ClassFiles
{
    public class Trades
    {
        public string Symbol { get; set; }
        public int exch { get; set; }
        public int buysell { get; set; }
        public int producttype { get; set; }
        public int qty { get; set; }
        public decimal price { get; set; }
        public decimal Ordprice { get; set; }
        public double orderno { get; set; }
        public int validity { get; set; }
        public string userremarks { get; set; }
        public string clientcode { get; set; }
        public string traderid { get; set; }
        public DateTime Createon { get; set; }
        public int ordstatus { get; set; }
        public DateTime Lastmodified { get; set; }
        public int isAdmin { get; set; }
        public int isMaintenance { get; set; }
        public int exectype { get; set; }
        public string macIP { get; set; }
    }
}