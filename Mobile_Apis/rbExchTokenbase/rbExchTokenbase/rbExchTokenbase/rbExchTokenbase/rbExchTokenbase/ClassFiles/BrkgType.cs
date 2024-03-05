using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rbExchTokenbase.ClassFiles
{
    public class BrkgType
    {
        public string clientCode { get; set; }
        public int mcx { get; set; } // 1 - Turnover wise , 2 - Lotwise
        public int nsefut { get; set; } // 1 - Turnover wise , 2 - Lotwise
        public int ncdex { get; set; } // 1 - Turnover wise , 2 - Lotwise
        public int nsecurr { get; set; } // 1 - Turnover wise , 2 - Lotwise
    }
}