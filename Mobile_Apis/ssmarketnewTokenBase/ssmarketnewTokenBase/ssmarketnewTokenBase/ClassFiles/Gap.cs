using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase.ClassFiles
{
    public class Gap
    {
        public string symbol { get; set; }
        public decimal Prev_Close { get; set; }
        public decimal open { get; set; }
        public decimal ltp { get; set; }
        public decimal percentchange { get; set; }
    }
}