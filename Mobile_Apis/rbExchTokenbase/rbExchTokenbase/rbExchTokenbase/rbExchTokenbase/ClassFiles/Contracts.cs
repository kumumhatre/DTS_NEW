using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rbExchTokenbase.ClassFiles
{
    public struct Contracts
    {
        public string symbol;
        public DateTime expiry;
        public int lotsize;
        public string measurelotsize;
        public int tickprice;
        public int exch;
        public int status;
        public string SymDesp;
        public string UserSymbol;
    }
}