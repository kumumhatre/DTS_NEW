using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTS110Tokenbase.Classfile
{
    public class Limits
    {
        public string clientcode { get; set; }
        public decimal cashmrgn { get; set; }
        public decimal turnoverlimit { get; set; }
        public decimal mtmlosslimit { get; set; }
        public int turnmulti { get; set; }
        public int mtmmulti { get; set; }
        public int breakup { get; set; }
        public int brkgtype { get; set; }
        public decimal mcxIntrdybrkg { get; set; }
        public decimal nsefutIntrdybrkg { get; set; }
        public decimal ncdexIntrdybrkg { get; set; }
        public decimal nsecurrIntrdybrkg { get; set; }
        public decimal nseoptIntrdybrkg { get; set; }
        public decimal mcxCnfbrkg { get; set; }
        public decimal nsefutCnfbrkg { get; set; }
        public decimal ncdexCnfbrkg { get; set; }
        public decimal nsecurrCnfbrkg { get; set; }
        public decimal nseoptCnfbrkg { get; set; }
        public int tradeattributes { get; set; }
        public int mrgntype { get; set; }
        public int isIntrasqoff { get; set; }
        public int IsMrgnsqoff { get; set; }
        public DateTime timestamp { get; set; }
        public int lotwisetype { get; set; }
        public int mcxlots { get; set; }
        public int ncxlots { get; set; }
        public int nsefutlots { get; set; }
        public int nsecurlots { get; set; }
        public string possitionValidity { get; set; }
        public string Productype { get; set; }
        public int McxBrkup { get; set; }
        public int NsefutBrkup { get; set; }
        public int NcdexBrkup { get; set; }
        public int NsecurBrkup { get; set; }
        public int brkupType { get; set; }
    }
}