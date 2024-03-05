using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase.ClassFiles
{
    public class VOLUP
    {
        public string symbol { get; set; }
        public double pre_volume { get; set; }
        public double curr_volume { get; set; }
        public decimal percent { get; set; }
        public double LTP { get; set; }
    }
}