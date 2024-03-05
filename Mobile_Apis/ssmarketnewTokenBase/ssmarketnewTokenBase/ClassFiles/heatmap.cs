using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase.ClassFiles
{
    public class heatmap
    {
        public string symbol { get; set; }
        public double Prev_ClosePrice { get; set; }
        public double LTP { get; set; }
        public decimal Difference { get; set; }
        public decimal Percentchange { get; set; }
    }
}