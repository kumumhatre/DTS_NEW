using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTS110Tokenbase.Classfile
{
    public struct buysellnetpospfls
    {
        public int buy_sell;
        public int Qty;
        public decimal Commision;
        public decimal Comm_Tax;
        public decimal margin;
        public decimal p_l;
        public decimal p_ltax;
        public int BQty;
        public int SQty;
        public decimal buyprice;
        public decimal sellprice;
        public string symbol;
        public decimal UnrealisedP_l;
        public double TurnoverUtilised;
    };
}