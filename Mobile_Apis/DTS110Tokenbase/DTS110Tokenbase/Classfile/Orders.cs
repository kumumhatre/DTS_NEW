using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTS110Tokenbase.Classfile
{
    public class Orders
    {
        //Symbol,Exch,BuySell,Qty,OrdPrice,Orderno,Validity,CreateOn,OrdStatus,Productype,Price
        public string Symbol { get; set; }
        public string Exch { get; set; }
        public string BuySell { get; set; }
        public int Qty { get; set; }
        public double OrdPrice { get; set; }
        public string Orderno { get; set; }
        public string Validity { get; set; }
        public string CreateOn { get; set; }
        public string OrdStatus { get; set; }
        public string Productype { get; set; }
        public double Price { get; set; }
    }
    public struct Order
    {
        public int orderid;
        public string symbol;
        public int BuySell;
        public int ExchangeTypeID;
        public int ProductType;
        public int Ordeqty;
        public decimal OrdePrice;
        public double OrderNo;
        public int ValidityType;
        public string UserRemark;
        public string accountNo;
        public string TraderId;
        public string CreatedOn;
        public string LastModified;
        public int Orderstatus;
        public int isAdmin;
        public int isMaintenance;
        public string AccName;
        public int Exectype;
        public decimal ExecPrice;
    };
    public class Trade
    {
        //Symbol,Exch,BuySell,Qty,OrdPrice,Orderno,Validity,CreateOn,OrdStatus,Productype,Price
        public string Symbol { get; set; }
        public string Exch { get; set; }
        public string BuySell { get; set; }
        public int Qty { get; set; }
        public double OrdPrice { get; set; }
        public string Orderno { get; set; }
        public string Validity { get; set; }
        public string CreateOn { get; set; }
        public double Price { get; set; }
        public string status { get; set; }
    }
}