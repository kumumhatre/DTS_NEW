using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTS110Tokenbase
{
    public class FRatesResponse
    {
        public IList<dynamic> data { get; set; }
        public CommonResponse description { get; set; }
    }
    public class FRatesResponse1
    {
        public IList<dynamic> data { get; set; }
        public IList<dynamic> TopPanel { get; set; }
        public CommonResponse description { get; set; }
    }
}