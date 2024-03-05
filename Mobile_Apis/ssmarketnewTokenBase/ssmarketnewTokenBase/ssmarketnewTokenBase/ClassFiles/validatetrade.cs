using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase.ClassFiles
{
    public class validatetrade
    {
        public string clientcode { get; set; }
        public string symbol { get; set; }
        public string operation { get; set; }
        public string qty { get; set; }
        public string price { get; set; }
        public string validity { get; set; }
        public string ordertype { get; set; }
        public string macip { get; set; }
        public bool IsModify { get; set; }
    }
}