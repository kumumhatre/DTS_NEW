using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase.ClassFiles
{
    public class realtimepivots
    {
        public string pivotname { get; set; }
        public string symbol { get; set; }
        public string levels { get; set; }

        public string price { get; set; }
        public DateTime Timestamp { get; set; }
    }
}