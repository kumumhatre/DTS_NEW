using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rbExchTokenbase.ClassFiles
{
    public class Alert
    {
        public string id { get; set; }
        public string clientcode { get; set; }
        public string symbol { get; set; }
        public string condition { get; set; }
        public string price { get; set; }
        public string timestamp { get; set; }
    }
}