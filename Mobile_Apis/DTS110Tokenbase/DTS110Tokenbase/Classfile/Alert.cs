using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTS110Tokenbase.Classfile
{
    public class Alert
    {
        public string id { get; set; }
        public string clientcode { get; set; }
        public string symbol { get; set; }
        public string condtion { get; set; }
        public string price { get; set; }
        public string timestamp { get; set; }
    }
}