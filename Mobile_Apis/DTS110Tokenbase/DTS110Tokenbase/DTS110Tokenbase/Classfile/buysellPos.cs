using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTS110Tokenbase.Classfile
{
    public struct buysellPos
    {
        public string symbol;
        public int BQty;
        public int SQty;
        public double price;
        public double buyprice;
        public double sellprice;
        public double accountNo;
        public int IntraBQty;
        public int IntraSQty;
        public double IntraBPrice;
        public double IntraSPrice;
    };
}