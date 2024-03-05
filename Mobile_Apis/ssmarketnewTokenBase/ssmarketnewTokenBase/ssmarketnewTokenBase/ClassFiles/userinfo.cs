using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase.ClassFiles
{
    public class userinfo
    {
        public string message { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string email_id { get; set; }
        public string exchanhges { get; set; }
        public string clientcode { get; set; }
        public Int32 userstatus { get; set; }
        public Int32 usertype { get; set; }
        public string createdby { get; set; }
        public string validity { get; set; }
        public string producttype { get; set; }
        public int oddLot { get; set; }
    }
    public class User_info
    {
        public string name { get; set; }
        public string clientcode { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public DateTime regdate { get; set; }
        public int userstatus { get; set; }
        public int marginstatus { get; set; }
        public int productype { get; set; }
        public int usertype { get; set; }
        public string createdby { get; set; }
        public string exchange { get; set; }
        public DateTime Lastlogin { get; set; }
        public int pivots { get; set; }
        public int stockperform { get; set; }
        public int charts { get; set; }
        public int offset { get; set; }
        public string mappedclients { get; set; }
        public int oddlot { get; set; }
        public string BlastPassword { get; set; }
        public string AppName { get; set; }
        public int isModTrd { get; set; }
        public byte[] imagedata { get; set; }
        public int DApflsPercent { get; set; }
        public string emailid { get; set; }
        public Int64 Mobno { get; set; }
        public int isSendMail { get; set; }
        public string OffsetExch { get; set; }
        public int isHLTrading { get; set; }
        public string HighLowExch { get; set; }
    }
}