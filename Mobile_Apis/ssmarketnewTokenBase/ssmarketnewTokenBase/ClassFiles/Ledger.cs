using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase.ClassFiles
{
    public class Ledger
    {
        public string accountno;
        public double orderno;
        public int buyqty;
        public double buyprice;
        public int sellqty;
        public double sellprice;
        public DateTime OpenTime;
        public DateTime CloseTime;
        public decimal comm;
        public double profitLoss;
        public decimal NettPl;
        public string symbol;
    }
}