using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase.ClassFiles
{
    public class OHLC
    {
        public string symbol;
        public DateTime FeedTime;
        public decimal Open;
        public decimal High;
        public decimal Low;
        public decimal CLose;
        public long volume;
    }
}