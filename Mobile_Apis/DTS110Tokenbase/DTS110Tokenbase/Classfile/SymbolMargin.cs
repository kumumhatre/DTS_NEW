using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTS110Tokenbase.Classfile
{
    public struct SymbolMargin
    {
        public string clientcode;
        public string symbol;
        public int intramrgn;
        public int intrabrkg;
        public int delvmrgn;
        public int delvbrkg;
        public int totlots;
        public int brkupQty;
    }
}