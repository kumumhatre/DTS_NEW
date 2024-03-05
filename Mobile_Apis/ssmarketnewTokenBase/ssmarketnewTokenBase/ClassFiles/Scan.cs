using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase.ClassFiles
{
    public class Scan
    {
        public string symbol { get; set; }
        public decimal open { get; set; }
        public decimal high { get; set; }
        public decimal ltp { get; set; }
        public decimal percentchange { get; set; }
    }
}