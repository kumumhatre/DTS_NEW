using DTS110Tokenbase.Classfile;
using DTS110Tokenbase.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace DTS110Tokenbase
{
    public class Utils
    {
        #region[sqlconnection]
        public static SqlConnection conn = new SqlConnection("Data Source = 173.212.229.110\\M4910, 1433; User ID = rocky; Initial Catalog = DTSNew; Password=Rocky@987;Integrated Security = false; MultipleActiveResultSets = True;Connect Timeout = 500000000");
        public static SqlConnection feedconn = new SqlConnection(@"Data Source=164.68.123.204\\M13504,1433;;User ID=Optimus;Initial Catalog=DTS;Password=Optimus@1010;Integrated Security=false;MultipleActiveResultSets = True;Connect Timeout=500000000");
        //public static SqlConnection feedconn = new SqlConnection("Data Source=103.5.45.221\\WIN-08CLDLUPJS4,1433;User ID=sa;Password=andhy@1234!;Initial Catalog=Feed;Integrated Security=false; MultipleActiveResultSets=True;Connect Timeout=500000000");
        #endregion

        public Dictionary<string, buysellnetpospfls> _NetProftLoss = new Dictionary<string, buysellnetpospfls>();
        public Dictionary<string, Contracts> _Symconctracts = new Dictionary<string, Contracts>();
        public Dictionary<string, BrkgType> _ClientBrokerageType = new Dictionary<string, BrkgType>();
        public Dictionary<string, Dictionary<string, Contracts>> _SymNameconctracts = new Dictionary<string, Dictionary<string, Contracts>>();
        public Dictionary<string, Feeds> _SymFeeds = new Dictionary<string, Feeds>();
        public Dictionary<int, SortedDictionary<string, Contracts>> _Exchconctracts = new Dictionary<int, SortedDictionary<string, Contracts>>();
        public Dictionary<string, Dictionary<string, SymbolMargin>> _ClientSymMrgn = new Dictionary<string, Dictionary<string, SymbolMargin>>();
        public List<string> _SubscribedSymbols = new List<string>();
        public Dictionary<string, Limits> _ClientLimits = new Dictionary<string, Limits>();
        public List<userinfo> _userInfo = new List<userinfo>();

        #region[encryptipondescrypton]
        public string Decryptdata(string encryptpwd)
        {
            string decryptpwd = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptpwd);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }
        public string Encryptdata(string password)
        {
            string strmsg = string.Empty;
            byte[] encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }
        #endregion

        public bool VerfiyAppAccess(string username, string appname, string appkey)
        {
            using (SqlCommand cmd = new SqlCommand("VerifyAppKey", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@key", appkey);
                cmd.Parameters.AddWithValue("@appname", appname);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int res = reader.GetInt32(0);
                        if (res > 0)
                            return true;
                        else
                            return false;
                    }
                }
            }
            return false;
        }

        public int validateuserstatus(string clientcode)
        {
            int i = 1;
            using (SqlCommand cmd = new SqlCommand("ValidateUserstatus", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Clientcode", clientcode);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int res = reader.GetInt32(6);
                        if (res != 1)
                            return res;
                        //else
                         //   return i ;
                    }
                }
            }
            return i;
        }

        int _buysell = 0;
        public Dictionary<string, Dictionary<int, List<Trades>>> GetTradePosition(ref Dictionary<string, buysellPos> _BuySellAvgPos, string account, bool isIntraday)
        {
            Dictionary<string, Dictionary<int, List<Trades>>> _BuySellAvgPoss = new Dictionary<string, Dictionary<int, List<Trades>>>();
            if (account == null)
                return _BuySellAvgPoss;
            if (account.Length == 0)
                return _BuySellAvgPoss;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
             using (DataSet ds = new DataSet())
            {
                //using (SqlDataAdapter cmd = new SqlDataAdapter() { SelectCommand = new SqlCommand("[getTradeposForMobile]", conn),CommandType = CommandType.StoredProcedure })
                //{

                //}
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                try
                {
                    cmd = new SqlCommand("getTradeposForMobile", conn);
                    cmd.Parameters.Add(new SqlParameter("@clientcode", account));
                    cmd.Parameters.Add(new SqlParameter("@isintraday", false));
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand = cmd;
                    da.Fill(ds);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Trades objtrade = new Trades()
                        {
                            Symbol = ds.Tables[0].Rows[i].ItemArray[0].ToString(),
                            qty = Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[1]),
                            buysell = Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[2]),

                            Lastmodified = Convert.ToDateTime(ds.Tables[0].Rows[i].ItemArray[3]),
                            price = Convert.ToDecimal(ds.Tables[0].Rows[i].ItemArray[4]),
                            clientcode = ds.Tables[0].Rows[i].ItemArray[5].ToString(),
                            validity = Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[6])
                        };
                        _buysell = objtrade.buysell;
                        if (objtrade.validity == 3)
                            objtrade.validity = 1;

                        ManagePositions(ref _BuySellAvgPos, objtrade);

                    }

                }
                catch (Exception x)
                {
                    
                }
            }
            return _BuySellAvgPoss;
        }
        public static void ManagePositions(ref Dictionary<string, buysellPos> _BuySellAvgPos, Trades objtrd)
        {
            string key = string.Format("{0}_{1}_{2}", objtrd.Symbol, objtrd.clientcode, objtrd.validity);

            if (_BuySellAvgPos.ContainsKey(key))
            {
                buysellPos objpos = _BuySellAvgPos[key];
                if (objtrd.buysell == 1)
                {
                    double avgprice = ((objpos.buyprice * objpos.BQty) + (Convert.ToDouble(objtrd.price) * objtrd.qty)) / (objpos.BQty + objtrd.qty);
                    objpos.BQty += objtrd.qty;
                    objpos.buyprice = avgprice;
                }
                else
                {
                    double avgprice = ((objpos.sellprice * objpos.SQty) + (Convert.ToDouble(objtrd.price) * objtrd.qty)) / (objpos.SQty + objtrd.qty);
                    objpos.SQty += objtrd.qty;
                    objpos.sellprice = avgprice;
                }
                _BuySellAvgPos[key] = objpos;
            }
            else
            {
                buysellPos objpos = new buysellPos();
                objpos.symbol = objtrd.Symbol;
                if (objtrd.buysell == 1)
                {
                    objpos.BQty = objtrd.qty;
                    objpos.SQty = 0;
                    objpos.buyprice = Convert.ToDouble(objtrd.price);
                }
                else
                {
                    objpos.BQty = 0;
                    objpos.SQty = objtrd.qty;
                    objpos.sellprice = Convert.ToDouble(objtrd.price);
                }
                _BuySellAvgPos.Add(key, objpos);
            }
        }

        public Dictionary<string, buysellnetpospfls> ProcessProfitLoss1(Dictionary<string, buysellPos> _BuySellAvgPos, SqlConnection Tradeconn, string clientcode)
        {
            Dictionary<string, buysellnetpospfls> _NetProftLoss = new Dictionary<string, buysellnetpospfls>();
            Limits objlimits = GetLimits(clientcode, Tradeconn);
            User_info objuserinfo = GetUserinfo(clientcode, Tradeconn);
            foreach (var items in _BuySellAvgPos)
            {
                buysellnetpospfls objnetpos = new buysellnetpospfls();
                string key = items.Key;
                string[] data = items.Key.Split('_');
                string symbol = data[0];
                string account = data[1];
                int Validity = Convert.ToInt32(data[2]);
                buysellPos objpos = items.Value;
                Contracts objcon = GetContract(symbol, Tradeconn);
                decimal Turnbrkg = GetTurnoverBrkg(objcon, objlimits, Validity);
                int totalqty = objpos.BQty + objpos.SQty;
                if (objcon.exch == 2 || objcon.exch == 5)
                    totalqty = (totalqty / objcon.lotsize);
                objnetpos.symbol = objpos.symbol;
                objnetpos.BQty = objpos.BQty;
                objnetpos.SQty = objpos.SQty;
                objnetpos.buyprice = Decimal.Round(Convert.ToDecimal(objpos.buyprice), 2);
                objnetpos.sellprice = Decimal.Round(Convert.ToDecimal(objpos.sellprice), 2);
                string[] symboldata = objnetpos.symbol.Split('(');
                Feeds objfeed = GetDBFeed(symbol, feedconn);
                SymbolMargin objmrgn = GetSymbolwiseMrgn(objuserinfo, symbol, Tradeconn);
                if (objnetpos.BQty > objnetpos.SQty)
                {
                    int net = objnetpos.BQty - objnetpos.SQty;
                    int lotwiseNetqty = net;
                    if (objcon.exch == 2 || objcon.exch == 5)
                        lotwiseNetqty = (net / objcon.lotsize);
                    decimal diffltp = objfeed.ltp - objnetpos.buyprice; // Bid
                    if (objfeed.ltp > 0)
                        if (objcon.exch == 2 || objcon.exch == 5)
                            objnetpos.UnrealisedP_l = Decimal.Round(diffltp * net, 2);
                        else
                            objnetpos.UnrealisedP_l = Decimal.Round(diffltp * net * objcon.lotsize, 2);
                    objnetpos.buy_sell = 1;
                    objnetpos.Qty = objnetpos.BQty - objnetpos.SQty;
                    if (objcon.exch == 2 || objcon.exch == 5)
                        objnetpos.TurnoverUtilised += Math.Round(net * Convert.ToDouble(objnetpos.buyprice));
                    else
                        objnetpos.TurnoverUtilised += Math.Round(net * Convert.ToDouble(objnetpos.buyprice) * objcon.lotsize);

                    if (Validity == 1)
                    {
                        objnetpos.margin = Convert.ToDecimal(lotwiseNetqty * objmrgn.delvmrgn);
                        if (objlimits.brkgtype == 2)
                            objnetpos.Commision = Convert.ToDecimal(totalqty * objmrgn.delvbrkg);
                        else
                        {
                            if (objnetpos.SQty > 0)
                            {
                                if (objcon.exch == 2 || objcon.exch == 5)
                                    objnetpos.Commision = Decimal.Round(((objnetpos.SQty * objnetpos.sellprice) * Turnbrkg) / 100, 2);
                                else
                                    objnetpos.Commision = Decimal.Round(((objnetpos.SQty * objnetpos.sellprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                            }
                            if (objcon.exch == 2 || objcon.exch == 5)
                                objnetpos.Commision += Decimal.Round(((objnetpos.BQty * objnetpos.buyprice) * Turnbrkg) / 100, 2);
                            else
                                objnetpos.Commision += Decimal.Round(((objnetpos.BQty * objnetpos.buyprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                        }

                    }
                    else
                    {
                        objnetpos.margin = Convert.ToDecimal(lotwiseNetqty * objmrgn.intramrgn);
                        if (objlimits.brkgtype == 2)
                            objnetpos.Commision = Convert.ToDecimal(totalqty * objmrgn.intrabrkg);
                        else
                        {
                            if (objnetpos.SQty > 0)
                            {
                                if (objcon.exch == 2 || objcon.exch == 5)
                                    objnetpos.Commision = Decimal.Round(((objnetpos.SQty * objnetpos.sellprice) * Turnbrkg) / 100, 2);
                                else
                                    objnetpos.Commision = Decimal.Round(((objnetpos.SQty * objnetpos.sellprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                            }
                            if (objcon.exch == 2 || objcon.exch == 5)
                                objnetpos.Commision += Decimal.Round(((objnetpos.BQty * objnetpos.buyprice) * Turnbrkg) / 100, 2);
                            else
                                objnetpos.Commision += Decimal.Round(((objnetpos.BQty * objnetpos.buyprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                        }
                    }

                    if (objnetpos.SQty > 0)
                    {
                        if (objcon.exch == 2 || objcon.exch == 5)
                            objnetpos.p_l = Decimal.Round((objnetpos.sellprice - objnetpos.buyprice) * objnetpos.SQty, 2);
                        else
                            objnetpos.p_l = Decimal.Round((objnetpos.sellprice - objnetpos.buyprice) * objnetpos.SQty * objcon.lotsize, 2);
                    }

                }
                else if (objnetpos.SQty > objnetpos.BQty)
                {
                    int net = objnetpos.SQty - objnetpos.BQty;
                    int lotwiseNetqty = net;
                    if (objcon.exch == 2 || objcon.exch == 5)
                        lotwiseNetqty = (net / objcon.lotsize);
                    decimal diffltp = objnetpos.sellprice - objfeed.ltp;//Ask
                    if (objfeed.ltp > 0)
                        if (objcon.exch == 2 || objcon.exch == 5)
                            objnetpos.UnrealisedP_l = Decimal.Round(diffltp * net, 2);
                        else
                            objnetpos.UnrealisedP_l = Decimal.Round(diffltp * net * objcon.lotsize, 2);
                    objnetpos.buy_sell = 2;
                    objnetpos.Qty = objnetpos.SQty - objnetpos.BQty;
                    if (objcon.exch == 2 || objcon.exch == 5)
                        objnetpos.TurnoverUtilised += Math.Round(net * Convert.ToDouble(objnetpos.sellprice));
                    else
                        objnetpos.TurnoverUtilised += Math.Round(net * Convert.ToDouble(objnetpos.sellprice) * objcon.lotsize);
                    if (Validity == 1)
                    {
                        objnetpos.margin = Convert.ToDecimal(lotwiseNetqty * objmrgn.delvmrgn);
                        if (objlimits.brkgtype == 2)
                            objnetpos.Commision = Convert.ToDecimal(totalqty * objmrgn.delvbrkg);
                        else
                        {
                            if (objnetpos.BQty > 0)
                            {
                                if (objcon.exch == 2 || objcon.exch == 5)
                                    objnetpos.Commision = Decimal.Round(((objnetpos.BQty * objnetpos.buyprice) * Turnbrkg) / 100, 2);
                                else
                                    objnetpos.Commision = Decimal.Round(((objnetpos.BQty * objnetpos.buyprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                            }
                            if (objcon.exch == 2 || objcon.exch == 5)
                                objnetpos.Commision += Decimal.Round(((objnetpos.SQty * objnetpos.sellprice) * Turnbrkg) / 100, 2);
                            else
                                objnetpos.Commision += Decimal.Round(((objnetpos.SQty * objnetpos.sellprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                        }

                    }
                    else
                    {
                        objnetpos.margin = Convert.ToDecimal(lotwiseNetqty * objmrgn.intramrgn);
                        if (objlimits.brkgtype == 2)
                            objnetpos.Commision = Convert.ToDecimal(totalqty * objmrgn.intrabrkg);
                        else
                        {
                            if (objnetpos.BQty > 0)
                            {
                                if (objcon.exch == 2 || objcon.exch == 5)
                                    objnetpos.Commision = Decimal.Round(((objnetpos.BQty * objnetpos.buyprice) * Turnbrkg) / 100, 2);
                                else
                                    objnetpos.Commision = Decimal.Round(((objnetpos.BQty * objnetpos.buyprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                            }
                            if (objcon.exch == 2 || objcon.exch == 5)
                                objnetpos.Commision += Decimal.Round(((objnetpos.SQty * objnetpos.sellprice) * Turnbrkg) / 100, 2);
                            else
                                objnetpos.Commision += Decimal.Round(((objnetpos.SQty * objnetpos.sellprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                        }
                    }
                    if (objnetpos.BQty > 0)
                    {
                        if (objcon.exch == 2 || objcon.exch == 5)
                            objnetpos.p_l = Decimal.Round((objnetpos.sellprice - objnetpos.buyprice) * objnetpos.BQty, 2);
                        else
                            objnetpos.p_l = Decimal.Round((objnetpos.sellprice - objnetpos.buyprice) * objnetpos.BQty * objcon.lotsize, 2);
                    }
                }
                else
                {
                    double avgprice = Math.Round((Convert.ToDouble(objnetpos.buyprice) + Convert.ToDouble(objnetpos.sellprice)) / 2, 2);

                    if (objlimits.brkgtype == 2)
                        if (Validity == 2)
                            objnetpos.Commision = Convert.ToDecimal(totalqty * objmrgn.delvbrkg);
                        else
                            objnetpos.Commision = Convert.ToDecimal(totalqty * objmrgn.intrabrkg);
                    else
                    {
                        int netqtyy = objnetpos.BQty + objnetpos.SQty;
                        if (objcon.exch == 2 || objcon.exch == 5)
                            objnetpos.Commision = Decimal.Round(((netqtyy * Convert.ToDecimal(avgprice)) * Turnbrkg) / 100, 2);
                        else
                            objnetpos.Commision = Decimal.Round(((netqtyy * Convert.ToDecimal(avgprice) * objcon.lotsize) * Turnbrkg) / 100, 2);
                    }

                    if (objcon.exch == 2 || objcon.exch == 5)
                        objnetpos.p_l = Decimal.Round((objnetpos.sellprice - objnetpos.buyprice) * objnetpos.BQty, 2);
                    else
                        objnetpos.p_l = Decimal.Round((objnetpos.sellprice - objnetpos.buyprice) * objnetpos.BQty * objcon.lotsize, 2);
                }

                try
                {
                    if (!_NetProftLoss.ContainsKey(key))
                        _NetProftLoss.Add(key, objnetpos);
                }
                catch { }



            }
            return _NetProftLoss;
        }
        
        public Limits GetLimits(string clientcode, SqlConnection conn)
        {
            Limits objlimits = new Limits();
            using (var cmd = new SqlCommand("Getlimits", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@clientcode", clientcode);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(1))
                            objlimits.clientcode = reader.GetString(1);

                        if (!reader.IsDBNull(2))
                            objlimits.cashmrgn = Convert.ToDecimal(reader.GetValue(2)) / 100;

                        if (!reader.IsDBNull(3))
                            objlimits.turnoverlimit = Convert.ToDecimal(reader.GetValue(3)) / 100;

                        if (!reader.IsDBNull(4))
                            objlimits.mtmlosslimit = Convert.ToDecimal(reader.GetValue(4)) / 100;

                        if (!reader.IsDBNull(5))
                            objlimits.turnmulti = reader.GetInt32(5);

                        if (!reader.IsDBNull(6))
                            objlimits.mtmmulti = Convert.ToInt32(reader.GetValue(6));

                        if (!reader.IsDBNull(7))
                            objlimits.breakup = reader.GetInt32(7);

                        if (!reader.IsDBNull(8))
                            objlimits.brkgtype = reader.GetInt32(8);

                        if (!reader.IsDBNull(9))
                            objlimits.mcxIntrdybrkg = Convert.ToDecimal(reader.GetValue(9));

                        if (!reader.IsDBNull(10))
                            objlimits.nsefutIntrdybrkg = Convert.ToDecimal(reader.GetValue(10));

                        if (!reader.IsDBNull(11))
                            objlimits.ncdexIntrdybrkg = Convert.ToDecimal(reader.GetValue(11));

                        if (!reader.IsDBNull(12))
                            objlimits.nsecurrIntrdybrkg = Convert.ToDecimal(reader.GetValue(12));

                        if (!reader.IsDBNull(13))
                            objlimits.mcxCnfbrkg = Convert.ToDecimal(reader.GetValue(13));

                        if (!reader.IsDBNull(14))
                            objlimits.nsefutCnfbrkg = Convert.ToDecimal(reader.GetValue(14));

                        if (!reader.IsDBNull(15))
                            objlimits.ncdexCnfbrkg = Convert.ToDecimal(reader.GetValue(15));

                        if (!reader.IsDBNull(16))
                            objlimits.nsecurrCnfbrkg = Convert.ToDecimal(reader.GetValue(16));

                        if (!reader.IsDBNull(17))
                            objlimits.tradeattributes = reader.GetInt32(17);

                        if (!reader.IsDBNull(18))
                            objlimits.mrgntype = reader.GetInt32(18);

                        if (!reader.IsDBNull(19))
                            objlimits.isIntrasqoff = reader.GetInt32(19);

                        if (!reader.IsDBNull(20))
                            objlimits.IsMrgnsqoff = reader.GetInt32(20);

                        if (!reader.IsDBNull(21))
                            objlimits.timestamp = Convert.ToDateTime(reader.GetValue(21));

                        if (!reader.IsDBNull(22))
                            objlimits.lotwisetype = reader.GetInt32(22);

                        if (!reader.IsDBNull(23))
                            objlimits.mcxlots = reader.GetInt32(23);

                        if (!reader.IsDBNull(24))
                            objlimits.ncxlots = reader.GetInt32(24);

                        if (!reader.IsDBNull(25))
                            objlimits.nsefutlots = reader.GetInt32(25);

                        if (!reader.IsDBNull(26))
                            objlimits.nsecurlots = reader.GetInt32(26);

                        if (!reader.IsDBNull(27))
                            objlimits.possitionValidity = reader.GetString(27);
                        else
                            objlimits.possitionValidity = string.Empty;

                        if (!reader.IsDBNull(28))
                            objlimits.Productype = reader.GetString(28);
                        else
                            objlimits.Productype = string.Empty;

                        if (!reader.IsDBNull(29))
                            objlimits.McxBrkup = reader.GetInt32(29);

                        if (!reader.IsDBNull(30))
                            objlimits.NsefutBrkup = reader.GetInt32(30);

                        if (!reader.IsDBNull(31))
                            objlimits.NcdexBrkup = reader.GetInt32(31);

                        if (!reader.IsDBNull(32))
                            objlimits.NsecurBrkup = reader.GetInt32(32);

                        objlimits.brkupType = !reader.IsDBNull(35) ? reader.GetInt32(35) : 2;
                    }
                }
            }
            return objlimits;
        }
        public User_info GetUserinfo(string clientcode, SqlConnection conn)
        {
            using (SqlCommand cmd = new SqlCommand("GetUserinfoformobile", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@clientcode", clientcode);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return ExtractUserinfo(reader);
                    }
                }
            }
            return new User_info();
        }
        User_info ExtractUserinfo(SqlDataReader reader)
        {
            User_info objinfo = new User_info();
            if (!reader.IsDBNull(1))
                objinfo.name = reader.GetString(1);

            if (!reader.IsDBNull(2))
                objinfo.clientcode = reader.GetString(2);

            if (!reader.IsDBNull(3))
                objinfo.username = reader.GetString(3);

            if (!reader.IsDBNull(4))
                objinfo.password = reader.GetString(4);

            if (!reader.IsDBNull(5))
                objinfo.regdate = Convert.ToDateTime(reader.GetValue(5));

            if (!reader.IsDBNull(6))
                objinfo.userstatus = reader.GetInt32(6);

            if (!reader.IsDBNull(7))
                objinfo.marginstatus = reader.GetInt32(7);

            if (!reader.IsDBNull(8))
                objinfo.productype = reader.GetInt32(8);

            if (!reader.IsDBNull(9))
                objinfo.usertype = reader.GetInt32(9);

            if (!reader.IsDBNull(10))
                objinfo.createdby = reader.GetString(10);

            if (!reader.IsDBNull(11))
                objinfo.Lastlogin = Convert.ToDateTime(reader.GetValue(11));

            if (!reader.IsDBNull(12))
                objinfo.pivots = reader.GetInt32(12);

            if (!reader.IsDBNull(13))
                objinfo.stockperform = reader.GetInt32(13);

            if (!reader.IsDBNull(14))
                objinfo.charts = reader.GetInt32(14);

            if (!reader.IsDBNull(15))
                objinfo.exchange = reader.GetString(15);

            if (!reader.IsDBNull(16))
                objinfo.offset = reader.GetInt32(16);

            if (!reader.IsDBNull(17))
                objinfo.mappedclients = reader.GetString(17);

            if (!reader.IsDBNull(18))
                objinfo.oddlot = reader.GetInt32(18);

            if (!reader.IsDBNull(20))
                objinfo.AppName = reader.GetString(20);

            if (!reader.IsDBNull(23))
                objinfo.DApflsPercent = Convert.ToInt32(reader.GetValue(23));

            if (!reader.IsDBNull(24))
                objinfo.emailid = reader.GetString(24);

            if (!reader.IsDBNull(25))
                objinfo.Mobno = Convert.ToInt64(reader.GetValue(25));

            if (!reader.IsDBNull(26))
                objinfo.OffsetExch = reader.GetString(26);
            else
                objinfo.OffsetExch = string.Empty;

            if (!reader.IsDBNull(27))
                objinfo.isHLTrading = reader.GetInt32(27);

            if (!reader.IsDBNull(28))
                objinfo.HighLowExch = reader.GetString(28);
            else
                objinfo.HighLowExch = string.Empty;

            return objinfo;
        }

        public Contracts GetContract(string symbol, SqlConnection conn)
        {
            using (SqlCommand cmd = new SqlCommand("getContractsForMobile", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@symbol", symbol);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Contracts objcon = new Contracts();
                        if (!reader.IsDBNull(0))
                            objcon.symbol = reader.GetString(0);

                        if (!reader.IsDBNull(1))
                            objcon.expiry = Convert.ToDateTime(reader.GetValue(1));

                        if (!reader.IsDBNull(2))
                            objcon.lotsize = reader.GetInt32(2);

                        if (!reader.IsDBNull(4))
                            objcon.tickprice = reader.GetInt32(4);


                        if (!reader.IsDBNull(5))
                            objcon.exch = reader.GetInt32(5);

                        if (!reader.IsDBNull(7))
                            objcon.SymDesp = reader.GetString(7);

                        if (!reader.IsDBNull(8))
                            objcon.UserSymbol = reader.GetString(8);

                        return objcon;
                    }
                }
            }
            return new Contracts();
        }

        public decimal GetTurnoverBrkg(Contracts objcon, Limits objlimits, int postype)
        {
            switch (objcon.exch)
            {
                case 1:
                    if (postype == 1)
                        return objlimits.mcxCnfbrkg;
                    else
                        return objlimits.mcxIntrdybrkg;

                case 2:
                    if (postype == 1)
                        return objlimits.nsefutCnfbrkg;
                    else
                        return objlimits.nsefutIntrdybrkg;

                case 3:
                    if (postype == 1)
                        return objlimits.ncdexCnfbrkg;
                    else
                        return objlimits.ncdexIntrdybrkg;

                case 4:
                    if (postype == 1)
                        return objlimits.nsecurrCnfbrkg;
                    else
                        return objlimits.nsecurrIntrdybrkg;

                case 5:
                    if (postype == 1)
                        return objlimits.nsefutCnfbrkg;
                    else
                        return objlimits.nsefutIntrdybrkg;
            }
            return 0;
        }

        public Feeds GetDBFeed(string symbol, SqlConnection conn)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            using (SqlCommand cmdd = new SqlCommand("getDbFeedsForMobile", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmdd.Parameters.AddWithValue("@symbol", symbol);
                using (var reader = cmdd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return GetMarketData(reader, false);
                    }
                }
            }
            return new Feeds();
        }
        public Feeds GetMarketData(SqlDataReader reader, bool flag)
        {
            Feeds objfeeds = new Feeds();
            if (!reader.IsDBNull(0))
                objfeeds.symbol = reader.GetString(0);

            if (!reader.IsDBNull(1))
                objfeeds.bidqty = reader.GetInt32(1);

            if (!reader.IsDBNull(2))
                objfeeds.bid = Convert.ToDecimal(reader.GetValue(2));

            if (!reader.IsDBNull(3))
                objfeeds.ask = Convert.ToDecimal(reader.GetValue(3));

            if (!reader.IsDBNull(4))
                objfeeds.askqty = reader.GetInt32(4);

            if (!reader.IsDBNull(5))
                objfeeds.ltp = Convert.ToDecimal(reader.GetValue(5));

            if (!reader.IsDBNull(6))
                objfeeds.open = Convert.ToDecimal(reader.GetValue(6));

            if (!reader.IsDBNull(7))
                objfeeds.high = Convert.ToDecimal(reader.GetValue(7));

            if (!reader.IsDBNull(8))
                objfeeds.low = Convert.ToDecimal(reader.GetValue(8));

            if (!reader.IsDBNull(9))
                objfeeds.close = Convert.ToDecimal(reader.GetValue(9));

            if (!reader.IsDBNull(10))
                objfeeds.netchange = Convert.ToDecimal(reader.GetValue(10));

            if (!reader.IsDBNull(11))
                objfeeds.perchange = Convert.ToDecimal(reader.GetValue(11));

            if (!reader.IsDBNull(12))
            {
                objfeeds.ltt = Convert.ToDateTime(reader.GetValue(12));
            }

            objfeeds.volume = !reader.IsDBNull(15) ? Convert.ToDouble(reader.GetValue(15)) : 0;

            return objfeeds;
        }

        public SymbolMargin GetSymbolwiseMrgn(User_info objuserinfo, string symbol, SqlConnection conn)
        {
            SymbolMargin objmrgn = new SymbolMargin();
            using (var cmd = new SqlCommand("GetSymbolwiseMargin", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@clientcode", objuserinfo.clientcode);
                //cmd.Parameters.AddWithValue("@createdby", objuserinfo.createdby);
                cmd.Parameters.AddWithValue("@symbol", symbol);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(1))
                            objmrgn.symbol = reader.GetString(1);

                        if (!reader.IsDBNull(2))
                            objmrgn.intramrgn = reader.GetInt32(2);

                        if (!reader.IsDBNull(3))
                            objmrgn.intrabrkg = reader.GetInt32(3);

                        if (!reader.IsDBNull(4))
                            objmrgn.delvmrgn = reader.GetInt32(4);

                        if (!reader.IsDBNull(5))
                            objmrgn.delvbrkg = reader.GetInt32(5);

                        if (!reader.IsDBNull(6))
                            objmrgn.totlots = reader.GetInt32(6);

                        if (!reader.IsDBNull(8))
                            objmrgn.brkupQty = reader.GetInt32(8);
                    }
                }
            }

            return objmrgn;
        }
        public static string GetExch(int exch)
        {
            switch (exch)
            {
                case 1:
                    return "MCX";

                case 2:
                    return "NSEFUT";

                case 3:
                    return "NCDEX";

                case 4:
                    return "NSECURR";

                case 5:
                    return "NSEOPT";
            }
            return "";
        }

        public bool ValidatePendingOrder(double orderno, SqlConnection conn)
        {
            if (conn.State == ConnectionState.Open)
                using (var cmd = new SqlCommand("ValidatePendingOrder", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@orderno", orderno);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                int result = reader.GetInt32(0);
                                if (result >= 1)
                                    return true;
                                else
                                    return false;
                            }
                        }
                    }
                }
            return true;
        }

         public bool ValidateMarketTime(int exch, string symbol)
        {
            //DateTime nseStarttime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 15, 00);
            //DateTime nseEndtime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 30, 00);
            //DateTime mcxStrtTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 00, 00);
            //DateTime mcxEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 30, 00);
            //DateTime ncxStrtTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 00, 00);
            //DateTime ncxEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 00, 00);
            //DateTime nsecurrStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 00, 00);
            //DateTime nsecurrEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 00, 00);
            DateTime mcxStrtTime = DateTime.Now;
            DateTime mcxEndTime = DateTime.Now;
            DateTime nsecurrStartTime = DateTime.Now;
            DateTime nsecurrEndTime = DateTime.Now;
            DateTime nseStarttime = DateTime.Now;
            DateTime nseEndtime = DateTime.Now;
            DateTime ncxStrtTime = DateTime.Now;
            DateTime ncxEndTime = DateTime.Now;

            string time = string.Empty;
            SqlConnection conn1 = conn;
            if (conn1.State == ConnectionState.Open)
            {
                using (var cmd = new SqlCommand("GetMarketTiming", conn1) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@exch", exch);
                    try
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            int i = 0;
                            while (dr.Read())
                            {
                                time = dr.GetString(i);
                                i++;
                            }
                        }
                        string[] timearr = time.Split('-');
                        string starttime = timearr[0];
                        string endtime = timearr[1];
                        string shh = starttime.Substring(0, 2);
                        string smm = starttime.Substring(2, 2);
                        string sss = starttime.Substring(4, 2);
                        string ehh = endtime.Substring(0, 2);
                        string emm = endtime.Substring(2, 2);
                        string ess = endtime.Substring(4, 2);
                        if (exch == 1)
                        {
                            mcxStrtTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(shh), Convert.ToInt32(smm), Convert.ToInt32(sss));
                            mcxEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ehh), Convert.ToInt32(emm), Convert.ToInt32(ess));
                        }
                        if (exch == 2)
                        {
                            nseStarttime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(shh), Convert.ToInt32(smm), Convert.ToInt32(sss));
                            nseEndtime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ehh), Convert.ToInt32(emm), Convert.ToInt32(ess));
                        }
                        if (exch == 3)
                        {
                            ncxStrtTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(shh), Convert.ToInt32(smm), Convert.ToInt32(sss));
                            ncxEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ehh), Convert.ToInt32(emm), Convert.ToInt32(ess));
                        }
                        if (exch == 4)
                        {
                            nsecurrStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(shh), Convert.ToInt32(smm), Convert.ToInt32(sss));
                            nsecurrEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ehh), Convert.ToInt32(emm), Convert.ToInt32(ess));
                        }
                    }
                    catch { }
                }
            }

            bool flag = true;
            DateTime dtnow = DateTime.Now;
            int d = (int)dtnow.DayOfWeek;
            if (d != 6 && d != 0)
            {
                switch (exch)
                {
                    case 1:
                        if (dtnow < mcxStrtTime || dtnow > mcxEndTime)
                            flag = false;
                        break;

                    case 3:
                        if (symbol == string.Empty)
                        {
                            if (dtnow < ncxStrtTime || dtnow > ncxEndTime)
                                flag = false;
                        }
                        else if (symbol == "SYOREF")
                        {
                            if (dtnow < ncxStrtTime || dtnow > ncxEndTime)
                                flag = false;
                        }
                        else
                        {
                            if (dtnow < ncxStrtTime || dtnow > ncxEndTime)
                                flag = false;

                        }
                        break;

                    case 2:
                        if (dtnow < nseStarttime || dtnow > nseEndtime)
                            flag = false;
                        break;

                    case 4:
                        if (dtnow < nsecurrStartTime || dtnow > nsecurrEndTime)
                            flag = false;
                        break;
                }
            }
            else if (d == 6)
            {
                flag = false;//isSaturdayTrading;
            }
            else if (d == 0)
            {
                flag = false;
            }
            return flag;
        }

        public bool GetOrderDetails(SqlConnection conn, double orderno, ref Trades objtrd)
        {
            using (var cmd = new SqlCommand("getorderformobile", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@orderno", orderno);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        objtrd = new Trades();
                        objtrd.Symbol = reader.GetString(1);
                        objtrd.clientcode = reader.GetString(11);

                        objtrd.qty = reader.GetInt32(5);
                        objtrd.orderno = orderno;
                        objtrd.Createon = Convert.ToDateTime(reader.GetValue(13));
                        objtrd.Lastmodified = Convert.ToDateTime(reader.GetValue(14));
                        objtrd.buysell = reader.GetInt32(3);
                        objtrd.exch = reader.GetInt32(2);
                        objtrd.traderid = reader.GetString(12);
                        objtrd.producttype = reader.GetInt32(4);
                        objtrd.Ordprice = Convert.ToDecimal(reader.GetValue(6));
                        objtrd.exectype = reader.GetInt32(18);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool InsertPendingTrade(Trades objtrd, SqlConnection conn)
        {
            int res = 0;
            if (conn.State == ConnectionState.Open)
                using (var cmd = new SqlCommand("ExecutePending", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@accno", objtrd.clientcode);
                    cmd.Parameters.AddWithValue("@symbol", objtrd.Symbol);
                    cmd.Parameters.AddWithValue("@qty", objtrd.qty);
                    cmd.Parameters.AddWithValue("@orderno", objtrd.orderno);
                    cmd.Parameters.AddWithValue("@price", objtrd.price);
                    cmd.Parameters.AddWithValue("@buysell", objtrd.buysell);
                    cmd.Parameters.AddWithValue("@exch", objtrd.exch);
                    cmd.Parameters.AddWithValue("@tradedby", objtrd.traderid);
                    cmd.Parameters.AddWithValue("@exectype", objtrd.exectype);

                    try
                    {
                        res = cmd.ExecuteNonQuery();
                    }
                    catch { return false; }

                }

            if (res > 0)
                return true;
            else
                return false;
        }

        bool Getoffset(User_info objinfo, string symbol, ref double offset, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("GetOffset", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@clientcode", objinfo.clientcode);
                cmd.Parameters.AddWithValue("@symbol", symbol);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            offset = Convert.ToDouble(reader.GetValue(0));
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool ValidateOffset(Feeds objfeeds, int buysell, decimal price, int producttype, string account, string usersymbol, User_info objuserinfo, SqlConnection conn, ref string responseMsg)
        {
            decimal bidask = 0;
            double offset = 0;
            if (Getoffset(objuserinfo, usersymbol, ref offset, conn))
            {
                decimal ofset = Convert.ToDecimal(offset);
                switch (buysell)
                {
                    case 1:
                        switch (producttype)
                        {
                            case 1:
                                bidask = objfeeds.bid - ofset;
                                if (bidask < price)
                                {
                                    responseMsg = "Price Exceeding spread defined by Exchange";
                                    return false;
                                }
                                break;

                            case 2:
                                bidask = objfeeds.ask + ofset;
                                if (bidask > price)
                                {
                                    responseMsg = "Price Exceeding spread defined by Exchange";
                                    return false;
                                }
                                break;
                        }
                        break;

                    case 2:
                        switch (producttype)
                        {
                            case 1:
                                bidask = objfeeds.ask + ofset;
                                if (bidask > price)
                                {
                                    responseMsg = "Price Exceeding spread defined by Exchange";
                                    return false;
                                }
                                break;

                            case 2:
                                bidask = objfeeds.bid - ofset;
                                if (bidask < price)
                                {
                                    responseMsg = "Price Exceeding spread defined by Exchange";
                                    return false;
                                }
                                break;
                        }
                        break;
                }
            }
            else
            {
                responseMsg = "Offset Not Assigned to " + usersymbol + ".";
                return false;
            }

            return true;
        }

        public bool SqOffPos(string account, string symbol, int validity, int buysell, int qty, Contracts objcon, Feeds objfeeds, SqlConnection Tradeconn)
        {
            bool flag = false;
            string servertime = string.Empty;
            DateTime Time = DateTime.Now;
            Trades objord = new Trades()
            {
                exch = objcon.exch,
                Symbol = symbol,
                validity = validity,
                userremarks = string.Empty,
                producttype = 1,
                qty = qty,
                orderno = OrderNoGenerate(),
                clientcode = account,
                traderid = account,
                Createon = Time,
                Lastmodified = Time,
                ordstatus = 1,
                exectype = 2,
                buysell = buysell

            };

            if (buysell == 2)
            {
                objord.Ordprice = objfeeds.bid;
                objord.price = objfeeds.bid;
            }
            else
            {
                objord.Ordprice = objfeeds.ask;
                objord.price = objfeeds.ask;
            }
            if (objord.qty > 0 && objord.price > 0)
            {
                bool ordres = SaveOrder(objord, Tradeconn);
                if (ordres)
                {
                    bool trdres = SaveTrade(objord, Tradeconn);
                    if (trdres)
                    {
                        flag = trdres;
                    }
                }
            }

            return flag;
        }
        public static double OrderNoGenerate()
        {
            DateTime feedTime = DateTime.Now;
            String mnth = "";
            if (feedTime.Month.ToString().Length == 1)
                mnth = "0" + feedTime.Month;
            else
                mnth = feedTime.Month.ToString();

            String day = "";
            if (feedTime.Day.ToString().Length == 1)
                day = "0" + feedTime.Day;
            else
                day = feedTime.Day.ToString();

            int year = feedTime.Year - 2000;
            String dateStr = year + mnth + day;
            int date11 = Convert.ToInt32(dateStr);
            int time = 0;
            String minStr = "";
            if (feedTime.Minute.ToString().Length == 1)
                minStr = "0" + feedTime.Minute;
            else
                minStr = feedTime.Minute.ToString();

            String timeStr = feedTime.Hour + minStr + feedTime.Second + feedTime.Millisecond;

            if (feedTime.Second.ToString().Length == 1)
                time = Convert.ToInt32(timeStr) * 10;
            else
                time = Convert.ToInt32(timeStr);

            String OrderNo = dateStr + time;// +count;

            double OrdeNo = Convert.ToDouble(OrderNo);
            return OrdeNo;
        }

        public static bool SaveOrder(Trades objtrade, SqlConnection objconn)
        {
            SqlConnection conn = objconn;
            int res = 0;
            if (conn.State == ConnectionState.Open)
            {
                using (var cmd = new SqlCommand("SaveOrder", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@symbol", objtrade.Symbol);
                    cmd.Parameters.AddWithValue("@exch", objtrade.exch);
                    cmd.Parameters.AddWithValue("@buysell", objtrade.buysell);
                    cmd.Parameters.AddWithValue("@producttype", objtrade.producttype);
                    cmd.Parameters.AddWithValue("@qty", objtrade.qty);
                    cmd.Parameters.AddWithValue("@ordprice", objtrade.Ordprice);
                    cmd.Parameters.AddWithValue("@price", objtrade.price);
                    cmd.Parameters.AddWithValue("@orderno", objtrade.orderno);
                    cmd.Parameters.AddWithValue("@validity", objtrade.validity);
                    cmd.Parameters.AddWithValue("@userremarks", "Mobile");
                    cmd.Parameters.AddWithValue("@clientcode", objtrade.clientcode);
                    cmd.Parameters.AddWithValue("@traderid", objtrade.traderid);
                    cmd.Parameters.AddWithValue("@createon", objtrade.Createon.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    cmd.Parameters.AddWithValue("@lastmodified", objtrade.Lastmodified.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    cmd.Parameters.AddWithValue("@ordstatus", objtrade.ordstatus);
                    cmd.Parameters.AddWithValue("@isadmin", objtrade.isAdmin);
                    cmd.Parameters.AddWithValue("@ismaintenance", objtrade.isMaintenance);
                    cmd.Parameters.AddWithValue("@exectype", objtrade.exectype);
                    cmd.Parameters.AddWithValue("@ipaddress", objtrade.macIP);
                    try
                    {
                        res = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex) { }
                }
            }
            if (res > 0)
                return true;
            else
                return false;
        }

        public static bool SaveTrade(Trades objtrade, SqlConnection objconn)
        {
            int res = 0;
            if (objconn.State == ConnectionState.Open)
            {
                using (var cmd = new SqlCommand("SaveTrade", objconn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@clientcode", objtrade.clientcode);
                    cmd.Parameters.AddWithValue("@symbol", objtrade.Symbol);
                    cmd.Parameters.AddWithValue("@qty", objtrade.qty);
                    cmd.Parameters.AddWithValue("@orderno", objtrade.orderno);
                    cmd.Parameters.AddWithValue("@price", objtrade.price);
                    cmd.Parameters.AddWithValue("@lastmodified", objtrade.Lastmodified.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    cmd.Parameters.AddWithValue("@buysell", objtrade.buysell);
                    cmd.Parameters.AddWithValue("@exch", objtrade.exch);
                    cmd.Parameters.AddWithValue("@ipaddress", objtrade.macIP);
                    try
                    {
                        res = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex) { }
                }
            }
            if (res > 0)
                return true;
            else
                return false;
        }

        public bool ValidateContractStatus(string symbol, int flag, User_info objinfo, int exch, SqlConnection conn)
        {

            int _ConStatus = GetContractStatus(symbol, objinfo, conn);

            if (_ConStatus == flag)
                return false;

            return true;
        }

        public int GetContractStatus(string symbol, User_info objinfo, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("GetContractStatus", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@clientcode", objinfo.clientcode);
                cmd.Parameters.AddWithValue("@createdby", objinfo.createdby);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(2))
                        {
                            string symboll = reader.GetString(2);
                            if (symboll == symbol)
                                return reader.GetInt32(3);
                        }

                    }
                }
            }

            return 0;
        }

        public enum OrderType : int
        {
            RL = 1,
            SL = 2
        }

        public enum ValidityType : int
        {
            CARRYFORWARD = 1,
            DAY = 2,
            GTC
        }
        public string GetStringValidity(int Validitytype)
        {
            switch (Validitytype)
            {
                case 1:
                    return "CarryForward";

                case 2:
                    return "Day";

                case 3:
                    return "GTC";
            }
            return "";
        }

        public int getBreakup(Contracts objcon, Limits objclientlimits)
        {
            if (objcon.exch == 2)
            {
                return (objcon.lotsize * objclientlimits.NsefutBrkup);
            }
            else if (objcon.exch == 1)
                return objclientlimits.McxBrkup;
            else if (objcon.exch == 3)
                return objclientlimits.NcdexBrkup;
            else
                return objclientlimits.NsecurBrkup;
        }
        public static int GetRoundoff(int tick)
        {
            if (tick == 5 || tick == 10)
                return 2;
            else if (tick == 100 || tick == 200 || tick == 1000)
                return 0;
            else if (tick == 25000)
                return 4;
            else if (tick == 250)
                return 1;

            return 0;
        }

        public double GetDPRLimit(string symbol, string createdby, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("getdprlimit", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@symbol", symbol);
                cmd.Parameters.AddWithValue("@createdby", createdby);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(3))
                        {
                            return Convert.ToDouble(reader.GetValue(3));
                        }
                    }
                }
            }
            return 0;
        }

        public void GetPendingMargin(ref int SumPendIntraLot, ref int SumPendCnfLots, ref double PendingMargin, ref double TurnoverMrgn, string symbol, string clientcode,
       int exch, ref int ExchPendLots, Contracts objcon, SymbolMargin objmrgn, Limits objlimits, User_info _info, Feeds objfeed, ref int pendingqty, int buysell)
        {
            List<Order> _OrdersList = GetTradeOrders(2, clientcode);
            int buysell1 = buysell;
            foreach (var item in _OrdersList)
            {
                Order objord = item;
                if (objord.accountNo == clientcode.ToLower() || objord.accountNo == clientcode.ToUpper())
                {
                    if (objord.ExchangeTypeID == 2)
                    {
                        if (symbol == objord.symbol)
                        {
                            if (buysell1 == objord.BuySell)
                            {
                                if (_info.oddlot == 0)
                                    pendingqty += item.Ordeqty / objcon.lotsize;
                                else
                                    pendingqty += item.Ordeqty;
                            }

                        }
                    }
                    else
                    {
                        if (symbol == objord.symbol)
                        {
                            if (buysell1 == objord.BuySell)
                            {
                                pendingqty += item.Ordeqty;
                            }

                        }
                    }

                    int openqty = 0; int openbuysell = 0;//int buysell = 0;
                    GetSymbolwiseOpenPos(objcon.SymDesp, objord.ValidityType, clientcode, ref openqty, ref openbuysell, objmrgn, objlimits, objfeed, pendingqty);
                    //if (openbuysell == 1)
                    //    buysell = 1;
                    //else if (openbuysell == 2)
                    //    buysell = 2;
                    //else
                    Contracts objcon1 = GetContract(objord.symbol, conn);
                    buysell = objord.BuySell;

                    int qty = objord.Ordeqty;
                    if (objord.symbol != symbol)
                        buysell = objord.BuySell;
                    if ((objcon.exch == 2 || objcon.exch == 5) && _info.oddlot == 0)
                        qty = (objord.Ordeqty / objcon1.lotsize);

                    if (exch == objord.ExchangeTypeID)
                        ExchPendLots += qty;

                    if (objlimits.mrgntype == 1 || objlimits.mrgntype == 4 || objlimits.mrgntype == 6 || objlimits.mrgntype == 7)
                    {
                        if (objord.ExchangeTypeID == 2)
                            TurnoverMrgn += objord.Ordeqty * Convert.ToDouble(objord.OrdePrice);
                        else
                            TurnoverMrgn += objord.Ordeqty * Convert.ToDouble(objord.OrdePrice) * objcon1.lotsize;
                    }
                    if (objlimits.mrgntype != 1)
                    {
                        if (openbuysell != buysell)
                        {
                            if (openqty > qty)
                                qty = openqty - qty;
                            else if (openqty < qty)
                                qty = qty - openqty;
                            else
                                qty = openqty - qty;
                        }
                        if (objord.ValidityType == 1)
                        {
                            //if (openbuysell != objord.BuySell)
                            //{
                            //    if (openqty > qty)
                            //        qty = openqty - qty;
                            //    else if (openqty < qty)
                            //        qty = qty - openqty;
                            //    else
                            //        qty = openqty - qty;
                            //}
                            if (objord.symbol.ToUpper() == symbol.ToUpper())
                            {
                                //if (objord.BuySell == 1)
                                //    SumPendCnfLots += qty;
                                //if (objord.BuySell == 2)
                                //    SumPendCnfLots += qty;

                                if (objord.BuySell == buysell1)
                                    if ((objcon.exch == 2 || objcon.exch == 5) && _info.oddlot == 0)
                                    {
                                        int ordqty = (objord.Ordeqty / objcon.lotsize);
                                        SumPendCnfLots += ordqty;
                                    }
                                    else
                                    {
                                        SumPendCnfLots += objord.Ordeqty;
                                    }


                            }
                            PendingMargin += qty * objmrgn.delvmrgn;
                        }
                        else
                        {
                            if (objord.symbol.ToUpper() == symbol.ToUpper())
                            {
                                //if (objord.BuySell == 1)
                                //    SumPendCnfLots += qty;
                                //if (objord.BuySell == 2)
                                //    SumPendCnfLots += qty;

                                if (objord.BuySell == buysell1)
                                    if ((objcon.exch == 2 || objcon.exch == 5) && _info.oddlot == 0)
                                    {
                                        int ordqty = (objord.Ordeqty / objcon.lotsize);
                                        SumPendCnfLots += ordqty;
                                    }
                                    else
                                    {
                                        SumPendCnfLots += objord.Ordeqty;
                                    }

                            }

                            PendingMargin += qty * objmrgn.delvmrgn;
                        }
                    }

                }
            }
        }
        public List<Order> GetTradeOrders(int type, string clientcode)
        {
            // type 2 = pending Orders, 3 = Cancelled,1 = Trades
            List<Order> _OrderList = new List<Order>();

            string accounts = "'" + clientcode + "'";
            if (accounts == string.Empty)
                return _OrderList;

            //string query = String.Format("select Clientcode, Exch ,Symbol, Productype, BuySell,Qty, Validity, " +
            //    " LastModified, Userremarks,Traderid,OrderNo,OrdStatus, CreateOn, Price,ExecType,OrdPrice from " +
            //    " Orders  where  Clientcode in ({0}) and OrdStatus = {1} " +
            //    " and CreateOn > '{2}' Order by LastModified", accounts, type, DateTime.Now.ToString("yyyy-MM-dd 09:00:00"));
            //if (type == 1)
            //    query = String.Format("select Clientcode, Exch ,Symbol, Productype, BuySell,Qty, Validity, " +
            //    " LastModified, Userremarks,Traderid,OrderNo,OrdStatus, CreateOn, Price,ExecType,OrdPrice from " +
            //    " Orders  where  Clientcode in ({0}) and OrdStatus in (1,4,5,6) " +
            //    " and CreateOn > '{1}' Order by LastModified", accounts, DateTime.Now.ToString("yyyy-MM-dd 09:00:00"));
            try
            {
                if (conn.State == ConnectionState.Open)
                    using (var cmd = new SqlCommand("getTradeOrders", conn) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@type", type);
                        cmd.Parameters.AddWithValue("@clientcode", clientcode);
                        cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy-MM-dd 09:00:00"));
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Order objord = new Order();
                                if (!reader.IsDBNull(0))
                                    objord.accountNo = reader.GetString(0);

                                if (!reader.IsDBNull(1))
                                    objord.ExchangeTypeID = reader.GetInt32(1);

                                if (!reader.IsDBNull(2))
                                    objord.symbol = reader.GetString(2);

                                if (!reader.IsDBNull(3))
                                    objord.ProductType = reader.GetInt32(3);

                                if (!reader.IsDBNull(4))
                                    objord.BuySell = reader.GetInt32(4);

                                if (!reader.IsDBNull(5))
                                    objord.Ordeqty = reader.GetInt32(5);

                                if (!reader.IsDBNull(6))
                                    objord.ValidityType = reader.GetInt32(6);

                                if (!reader.IsDBNull(7))
                                    objord.LastModified = reader.GetValue(7).ToString();

                                if (!reader.IsDBNull(8))
                                    objord.UserRemark = reader.GetString(8);

                                if (!reader.IsDBNull(9))
                                    objord.TraderId = reader.GetString(9);

                                if (!reader.IsDBNull(10))
                                    objord.OrderNo = Convert.ToDouble(reader.GetValue(10));

                                if (!reader.IsDBNull(11))
                                    objord.Orderstatus = reader.GetInt32(11);

                                if (!reader.IsDBNull(12))
                                    objord.CreatedOn = reader.GetValue(12).ToString();

                                if (!reader.IsDBNull(13))
                                    objord.ExecPrice = Convert.ToDecimal(reader.GetValue(13));

                                if (!reader.IsDBNull(14))
                                    objord.Exectype = Convert.ToInt32(reader.GetValue(14));

                                if (!reader.IsDBNull(15))
                                    objord.OrdePrice = Convert.ToDecimal(reader.GetValue(15));

                                _OrderList.Add(objord);
                            }
                        }
                    }
            }
            catch
            {

            }
            return _OrderList;
        }

        private void GetSymbolwiseOpenPos(string symbol, int validity, string clientcode, ref int qty, ref int buysell, SymbolMargin objmrgn, Limits objlimits, Feeds objfeed, int pendingqty)
        {
            Dictionary<string, buysellPos> _BuySellAvgPos = new Dictionary<string, buysellPos>();
            Dictionary<string, Dictionary<int, List<Trades>>> _FIFOPos = GetTradePosition(ref _BuySellAvgPos, clientcode, false);
            Dictionary<string, buysellnetpospfls> _NetProftLoss = ProcessProfitLoss(_BuySellAvgPos, objmrgn, objlimits, objfeed);
            Dictionary<string, buysellnetpospfls> _NetPosPfLs = new Dictionary<string, buysellnetpospfls>(_NetProftLoss);
            foreach (var items in _NetPosPfLs)
            {
                string[] data = items.Key.Split('_');
                string account = data[1];
                int validityy = Convert.ToInt32(data[2]);
                buysellnetpospfls objnetpos = items.Value;
                if (symbol == objnetpos.symbol && account == clientcode)
                {
                    if (validity == validityy)
                    {
                        if (objnetpos.BQty > objnetpos.SQty)
                        {
                            qty = objnetpos.BQty - objnetpos.SQty;
                            buysell = 1;
                        }
                        else if (objnetpos.BQty < objnetpos.SQty)
                        {
                            qty = objnetpos.SQty - objnetpos.BQty;
                            buysell = 2;
                        }
                        if (objnetpos.BQty > pendingqty)
                        {
                            qty = objnetpos.BQty - pendingqty;
                            buysell = 1;
                        }
                        else if (objnetpos.BQty < pendingqty)
                        {
                            qty = objnetpos.SQty - pendingqty;
                            buysell = 2;
                        }
                    }
                }
            }
        }

        public Dictionary<string, buysellnetpospfls> ProcessProfitLoss(Dictionary<string, buysellPos> _BuySellAvgPos, SymbolMargin objmrgn, Limits objlimits, Feeds objfeed)//, Dictionary<string, decimal> _FIFOPL
        {
            Dictionary<string, buysellnetpospfls> _NetProftLoss = new Dictionary<string, buysellnetpospfls>();
            foreach (var items in _BuySellAvgPos)
            {
                buysellnetpospfls objnetpos = new buysellnetpospfls();
                string key = items.Key;
                string[] data = items.Key.Split('_');

                string account = data[1];
                User_info _info = GetUserinfo(account, conn);

                int Validity = Convert.ToInt32(data[2]);
                buysellPos objpos = items.Value;
                Contracts objcon = GetContract(objpos.symbol, conn);
                objmrgn = GetSymbolwiseMrgn(_info, objcon.symbol, conn);
                decimal Turnbrkg = GetTurnoverBrkg(objcon, objlimits, Validity);
                int totalqty = objpos.BQty + objpos.SQty;
                if (objcon.exch == 2 || objcon.exch == 5)
                    totalqty = (totalqty / objcon.lotsize);
                objnetpos.symbol = objpos.symbol;
                objnetpos.BQty = objpos.BQty;
                objnetpos.SQty = objpos.SQty;
                objnetpos.buyprice = Decimal.Round(Convert.ToDecimal(objpos.buyprice), 2);
                objnetpos.sellprice = Decimal.Round(Convert.ToDecimal(objpos.sellprice), 2);
                string[] symboldata = objnetpos.symbol.Split('(');

                if (objnetpos.BQty > objnetpos.SQty)
                {
                    int net = objnetpos.BQty - objnetpos.SQty;
                    int lotwiseNetqty = net;
                    if (objcon.exch == 2 || objcon.exch == 5)
                        lotwiseNetqty = (net / objcon.lotsize);
                    decimal diffltp = objfeed.ltp - objnetpos.buyprice; // Bid
                    if (objfeed.ltp > 0)
                        if (objcon.exch == 2 || objcon.exch == 5)
                            objnetpos.UnrealisedP_l = Decimal.Round(diffltp * net, 2);
                        else
                            objnetpos.UnrealisedP_l = Decimal.Round(diffltp * net * objcon.lotsize, 2);
                    objnetpos.buy_sell = 1;
                    objnetpos.Qty = objnetpos.BQty - objnetpos.SQty;
                    if (objcon.exch == 2 || objcon.exch == 5)
                        objnetpos.TurnoverUtilised += Math.Round(net * Convert.ToDouble(objnetpos.buyprice));
                    else
                        objnetpos.TurnoverUtilised += Math.Round(net * Convert.ToDouble(objnetpos.buyprice) * objcon.lotsize);

                    if (Validity == 1)
                    {
                        objnetpos.margin = Convert.ToDecimal(lotwiseNetqty * objmrgn.delvmrgn);
                        if (objlimits.brkgtype == 2)
                            objnetpos.Commision = Convert.ToDecimal(totalqty * objmrgn.delvbrkg);
                        else
                        {
                            if (objnetpos.SQty > 0)
                            {
                                if (objcon.exch == 2 || objcon.exch == 5)
                                    objnetpos.Commision = Decimal.Round(((objnetpos.SQty * objnetpos.sellprice) * Turnbrkg) / 100, 2);
                                else
                                    objnetpos.Commision = Decimal.Round(((objnetpos.SQty * objnetpos.sellprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                            }
                            if (objcon.exch == 2 || objcon.exch == 5)
                                objnetpos.Commision += Decimal.Round(((objnetpos.BQty * objnetpos.buyprice) * Turnbrkg) / 100, 2);
                            else
                                objnetpos.Commision += Decimal.Round(((objnetpos.BQty * objnetpos.buyprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                        }

                    }
                    else
                    {
                        objnetpos.margin = Convert.ToDecimal(lotwiseNetqty * objmrgn.intramrgn);
                        if (objlimits.brkgtype == 2)
                            objnetpos.Commision = Convert.ToDecimal(totalqty * objmrgn.intrabrkg);
                        else
                        {
                            if (objnetpos.SQty > 0)
                            {
                                if (objcon.exch == 2 || objcon.exch == 5)
                                    objnetpos.Commision = Decimal.Round(((objnetpos.SQty * objnetpos.sellprice) * Turnbrkg) / 100, 2);
                                else
                                    objnetpos.Commision = Decimal.Round(((objnetpos.SQty * objnetpos.sellprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                            }
                            if (objcon.exch == 2 || objcon.exch == 5)
                                objnetpos.Commision += Decimal.Round(((objnetpos.BQty * objnetpos.buyprice) * Turnbrkg) / 100, 2);
                            else
                                objnetpos.Commision += Decimal.Round(((objnetpos.BQty * objnetpos.buyprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                        }
                    }

                    if (objnetpos.SQty > 0)
                    {
                        if (objcon.exch == 2 || objcon.exch == 5)
                            objnetpos.p_l = Decimal.Round((objnetpos.sellprice - objnetpos.buyprice) * objnetpos.SQty, 2);
                        else
                            objnetpos.p_l = Decimal.Round((objnetpos.sellprice - objnetpos.buyprice) * objnetpos.SQty * objcon.lotsize, 2);
                    }

                }
                else if (objnetpos.SQty > objnetpos.BQty)
                {
                    int net = objnetpos.SQty - objnetpos.BQty;
                    int lotwiseNetqty = net;
                    if (objcon.exch == 2 || objcon.exch == 5)
                        lotwiseNetqty = (net / objcon.lotsize);
                    decimal diffltp = objnetpos.sellprice - objfeed.ltp;//Ask
                    if (objfeed.ltp > 0)
                        if (objcon.exch == 2 || objcon.exch == 5)
                            objnetpos.UnrealisedP_l = Decimal.Round(diffltp * net, 2);
                        else
                            objnetpos.UnrealisedP_l = Decimal.Round(diffltp * net * objcon.lotsize, 2);
                    objnetpos.buy_sell = 2;
                    objnetpos.Qty = objnetpos.SQty - objnetpos.BQty;
                    if (objcon.exch == 2 || objcon.exch == 5)
                        objnetpos.TurnoverUtilised += Math.Round(net * Convert.ToDouble(objnetpos.sellprice));
                    else
                        objnetpos.TurnoverUtilised += Math.Round(net * Convert.ToDouble(objnetpos.sellprice) * objcon.lotsize);
                    if (Validity == 1)
                    {
                        objnetpos.margin = Convert.ToDecimal(lotwiseNetqty * objmrgn.delvmrgn);
                        if (objlimits.brkgtype == 2)
                            objnetpos.Commision = Convert.ToDecimal(totalqty * objmrgn.delvbrkg);
                        else
                        {
                            if (objnetpos.BQty > 0)
                            {
                                if (objcon.exch == 2 || objcon.exch == 5)
                                    objnetpos.Commision = Decimal.Round(((objnetpos.BQty * objnetpos.buyprice) * Turnbrkg) / 100, 2);
                                else
                                    objnetpos.Commision = Decimal.Round(((objnetpos.BQty * objnetpos.buyprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                            }
                            if (objcon.exch == 2 || objcon.exch == 5)
                                objnetpos.Commision += Decimal.Round(((objnetpos.SQty * objnetpos.sellprice) * Turnbrkg) / 100, 2);
                            else
                                objnetpos.Commision += Decimal.Round(((objnetpos.SQty * objnetpos.sellprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                        }

                    }
                    else
                    {
                        objnetpos.margin = Convert.ToDecimal(lotwiseNetqty * objmrgn.intramrgn);
                        if (objlimits.brkgtype == 2)
                            objnetpos.Commision = Convert.ToDecimal(totalqty * objmrgn.intrabrkg);
                        else
                        {
                            if (objnetpos.BQty > 0)
                            {
                                if (objcon.exch == 2 || objcon.exch == 5)
                                    objnetpos.Commision = Decimal.Round(((objnetpos.BQty * objnetpos.buyprice) * Turnbrkg) / 100, 2);
                                else
                                    objnetpos.Commision = Decimal.Round(((objnetpos.BQty * objnetpos.buyprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                            }
                            if (objcon.exch == 2 || objcon.exch == 5)
                                objnetpos.Commision += Decimal.Round(((objnetpos.SQty * objnetpos.sellprice) * Turnbrkg) / 100, 2);
                            else
                                objnetpos.Commision += Decimal.Round(((objnetpos.SQty * objnetpos.sellprice * objcon.lotsize) * Turnbrkg) / 100, 2);
                        }
                    }
                    if (objnetpos.BQty > 0)
                    {
                        if (objcon.exch == 2 || objcon.exch == 5)
                            objnetpos.p_l = Decimal.Round((objnetpos.sellprice - objnetpos.buyprice) * objnetpos.BQty, 2);
                        else
                            objnetpos.p_l = Decimal.Round((objnetpos.sellprice - objnetpos.buyprice) * objnetpos.BQty * objcon.lotsize, 2);
                    }
                }
                else
                {
                    double avgprice = Math.Round((Convert.ToDouble(objnetpos.buyprice) + Convert.ToDouble(objnetpos.sellprice)) / 2, 2);

                    if (objlimits.brkgtype == 2)
                        if (Validity == 2)
                            objnetpos.Commision = Convert.ToDecimal(totalqty * objmrgn.delvbrkg);
                        else
                            objnetpos.Commision = Convert.ToDecimal(totalqty * objmrgn.intrabrkg);
                    else
                    {
                        int netqtyy = objnetpos.BQty + objnetpos.SQty;
                        if (objcon.exch == 2 || objcon.exch == 5)
                            objnetpos.Commision = Decimal.Round(((netqtyy * Convert.ToDecimal(avgprice)) * Turnbrkg) / 100, 2);
                        else
                            objnetpos.Commision = Decimal.Round(((netqtyy * Convert.ToDecimal(avgprice) * objcon.lotsize) * Turnbrkg) / 100, 2);
                    }

                    if (objcon.exch == 2 || objcon.exch == 5)
                        objnetpos.p_l = Decimal.Round((objnetpos.sellprice - objnetpos.buyprice) * objnetpos.BQty, 2);
                    else
                        objnetpos.p_l = Decimal.Round((objnetpos.sellprice - objnetpos.buyprice) * objnetpos.BQty * objcon.lotsize, 2);
                }

                try
                {
                    if (!_NetProftLoss.ContainsKey(key))
                        _NetProftLoss.Add(key, objnetpos);
                }
                catch { }



            }
            return _NetProftLoss;
        }

        public void GetMarginUtlised(Trades objtrd, User_info _info, ref int SumIntraLots, ref int SumCnfLots, ref double MarginUtilised, ref double ProfitLoss,
        ref int Totintralots, ref bool isclosepos, ref double RealisedPL, ref double TurnoverUtilised, ref int SymLots, ref int ExchLots, Feeds objfeed,
        SymbolMargin objmrgn, Limits objlimits, int pendinglots)
        {
            Dictionary<string, buysellPos> _BuySellAvgPos = new Dictionary<string, buysellPos>();
            Dictionary<string, Dictionary<int, List<Trades>>> _FIFOPos = GetTradePosition(ref _BuySellAvgPos, objtrd.clientcode, false);
            Dictionary<string, buysellnetpospfls> _NetProftLoss = ProcessProfitLoss(_BuySellAvgPos, objmrgn, objlimits, objfeed);
            Dictionary<string, buysellnetpospfls> _NetPosPfLs = new Dictionary<string, buysellnetpospfls>(_NetProftLoss);
            foreach (var items in _NetPosPfLs)
            {
                string[] data = items.Key.Split('_');
                string account = data[1];
                int validity = Convert.ToInt32(data[2]);
                buysellnetpospfls objnetpos = items.Value;
                Contracts objcon = GetContract(objnetpos.symbol, conn);
                decimal DiffLTP = 0;
                if (validity == 1)
                    Totintralots += objnetpos.BQty + objnetpos.SQty;

                if (objcon.exch == objtrd.exch)
                {
                    if ((objcon.exch == 2 || objcon.exch == 5) && _info.oddlot == 0)
                    {
                        int lots = objnetpos.Qty / objcon.lotsize;
                        ExchLots += lots;
                    }
                    else
                        ExchLots += objnetpos.Qty;
                }

                if (objtrd.validity == 1)
                    SumCnfLots += objnetpos.Qty;
                else
                    SumIntraLots += objnetpos.Qty;

                if (account == objtrd.clientcode)
                {
                    if (objnetpos.BQty > objnetpos.SQty)
                    {
                        int openqty = objnetpos.BQty - objnetpos.SQty;
                        if (objcon.exch == 2 || objcon.exch == 5)
                            TurnoverUtilised += openqty * Convert.ToDouble(objnetpos.buyprice);
                        else
                            TurnoverUtilised += openqty * objcon.lotsize * Convert.ToDouble(objnetpos.buyprice);
                    }
                    else if (objnetpos.BQty < objnetpos.SQty)
                    {
                        int openqty = objnetpos.SQty - objnetpos.BQty;
                        if (objcon.exch == 2 || objcon.exch == 5)
                            TurnoverUtilised -= openqty * Convert.ToDouble(objnetpos.sellprice);
                        else
                            TurnoverUtilised -= openqty * objcon.lotsize * Convert.ToDouble(objnetpos.sellprice);
                    }
                }

                if (objnetpos.symbol == objtrd.Symbol)
                {
                    if (objnetpos.BQty > objnetpos.SQty)
                    {
                        int openqty = objnetpos.BQty - objnetpos.SQty;
                        if (objcon.exch == 2 /*|| objcon.exch == 5*/ && _info.oddlot == 0)
                            SymLots += (openqty / objcon.lotsize);
                        else
                            SymLots += openqty;

                        if (/*openqty >= objtrd.qty && */objtrd.buysell == 2)
                            isclosepos = true;
                        else
                            isclosepos = false;
                    }
                    else if (objnetpos.BQty < objnetpos.SQty)
                    {

                        int openqty = objnetpos.SQty - objnetpos.BQty;
                        if (objcon.exch == 2 /*|| objcon.exch == 5*/ && _info.oddlot == 0)
                            SymLots += (openqty / objcon.lotsize);
                        else
                            SymLots += openqty;
                        if (/*openqty >= objtrd.qty &&*/ objtrd.buysell == 1)
                            isclosepos = true;
                        else
                            isclosepos = false;
                    }
                    else
                        isclosepos = false;
                    //if (objlimits.mrgntype == 2 && objlimits.lotwisetype == 3)
                    //{
                    //    if (isclosepos == true)
                    //    {
                    //        if (objcon.exch == 2 && _info.oddlot == 0)
                    //        {
                    //            int netposqty = objnetpos.Qty / objcon.lotsize;
                    //            int trdqty = objtrd.qty / objcon.lotsize;
                    //            int totallots = objmrgn.totlots + netposqty;
                    //            int tottrd = pendinglots + trdqty;
                    //            if (totallots < tottrd)
                    //            {
                    //                isclosepos = false;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //int totallots = objmrgn.totlots * 2;
                    //            int totallots = objmrgn.totlots + objnetpos.Qty;
                    //            int tottrd = pendinglots + objtrd.qty;
                    //            if (totallots < tottrd)
                    //            {
                    //                isclosepos = false;
                    //            }
                    //        }



                    //    }
                    //}
                    if (((objlimits.mrgntype == 2 || objlimits.mrgntype == 4 || objlimits.mrgntype == 5 || objlimits.mrgntype == 7) && objlimits.lotwisetype == 3) || ((objlimits.mrgntype == 2 || objlimits.mrgntype == 4 || objlimits.mrgntype == 5 || objlimits.mrgntype == 7) && objlimits.lotwisetype == 1) || ((objlimits.mrgntype == 2 || objlimits.mrgntype == 4 || objlimits.mrgntype == 5 || objlimits.mrgntype == 7) && objlimits.lotwisetype == 2))
                    {
                        int totlimitlots = 0;
                        if (objlimits.lotwisetype == 2)
                        {
                            if (objcon.exch == 1)
                                totlimitlots = objlimits.mcxlots;
                            if (objcon.exch == 2)
                                totlimitlots = objlimits.nsefutlots;
                            if (objcon.exch == 3)
                                totlimitlots = objlimits.ncxlots;
                            if (objcon.exch == 4)
                                totlimitlots = objlimits.nsecurlots;
                        }
                        if (objlimits.lotwisetype == 3)
                        {
                            totlimitlots = objmrgn.totlots;
                        }
                        if (objlimits.lotwisetype == 1)
                        {
                            if (objcon.exch == 1)
                                totlimitlots = GetDefaultExchwiselots(_info.createdby, objcon.exch);
                            if (objcon.exch == 2)
                                totlimitlots = GetDefaultExchwiselots(_info.createdby, objcon.exch);
                            if (objcon.exch == 3)
                                totlimitlots = GetDefaultExchwiselots(_info.createdby, objcon.exch);
                            if (objcon.exch == 4)
                                totlimitlots = GetDefaultExchwiselots(_info.createdby, objcon.exch);
                        }

                        if (isclosepos == true)
                        {
                            if (objcon.exch == 2 && _info.oddlot == 0)
                            {
                                int netposqty = objnetpos.Qty / objcon.lotsize;
                                int trdqty = objtrd.qty / objcon.lotsize;
                                //int marginqty= objmrgn.totlots / objcon.lotsize;
                                int totallots = totlimitlots + netposqty;
                                int tottrd = pendinglots + trdqty;
                                if (totallots < tottrd)
                                {
                                    isclosepos = false;
                                }
                            }
                            else
                            {
                                //int totallots = objmrgn.totlots * 2;
                                int totallots = totlimitlots + objnetpos.Qty;
                                int tottrd = pendinglots + objtrd.qty;
                                if (totallots < tottrd)
                                {
                                    isclosepos = false;
                                }
                            }



                        }
                    }
                }

                RealisedPL += Convert.ToDouble(objnetpos.p_l - objnetpos.Commision - objnetpos.Comm_Tax - objnetpos.p_ltax);
                MarginUtilised += Convert.ToDouble(objnetpos.margin);

                if (objnetpos.buy_sell == 1)
                    DiffLTP = objfeed.ltp - objnetpos.buyprice;
                else
                    DiffLTP = objnetpos.sellprice - objfeed.ltp;

                decimal OpenPL = Decimal.Round(DiffLTP * objnetpos.Qty * objcon.lotsize, 2);
                ProfitLoss += Convert.ToDouble(objnetpos.p_l - objnetpos.Commision - objnetpos.Comm_Tax + OpenPL - objnetpos.p_ltax);
            }
        }
        public int GetDefaultExchwiselots(string createdby, int exch)
        {
            int lots = 0;
            defaultexchwiselots dlots = new defaultexchwiselots();
            using (var cmd = new SqlCommand("[GetDefaultExchwiselots]", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@createdby", createdby);
                using (var reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {

                        dlots.mcxlots = reader.GetInt32(0);
                        dlots.nsefutlots = reader.GetInt32(1);
                        dlots.ncdexlots = reader.GetInt32(2);
                        dlots.nsecurrlots = reader.GetInt32(3);
                        dlots.nseoptlots = reader.GetInt32(4);
                    }
                }
            }
            if (exch == 1)
                lots = dlots.mcxlots;
            if (exch == 2)
                lots = dlots.nsefutlots;
            if (exch == 3)
                lots = dlots.ncdexlots;
            if (exch == 4)
                lots = dlots.nsecurrlots;
            if (exch == 5)
                lots = dlots.nseoptlots;

            return lots;
        }
      public  bool ValidateMargin(Trades objtrd, double TotTurnover, Limits objlimits, User_info objinfo, int totLots, double MarginUtilised, int SymbolLots, ref string responseMsg, SymbolMargin objSymMrgn)
        {
            switch (objlimits.mrgntype)
            {
                case 1:
                    if (TotTurnover > Convert.ToDouble(objlimits.turnoverlimit))//Turnover wise
                    {
                        responseMsg = "Turnover exceeding Turnover Margin.";
                        return false;
                    }
                    break;

                case 2:
                    switch (objlimits.lotwisetype)//Lotwise
                    {
                        case 1:
                            DEFAULTEXCHMRGN exchdata = getdata(objinfo.createdby);
                            switch (objtrd.exch)//Default Exchangewise Lotsize
                            {
                                case 1:
                                    if (totLots > exchdata.mcx)
                                    {
                                        responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + exchdata.mcx + "";
                                        return false;
                                        //objmain.DisplayMessage("Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + objmain.MCXlots + ".", 2);
                                        //objmain.insertSurveillanceMessages(objtrd.clientcode, "Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + objmain.MCXlots + ".", "");

                                    }
                                    break;

                                case 2:
                                    if (totLots > exchdata.nsefut)
                                    {
                                        responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + exchdata.nsefut + "";
                                        //  objmain.DisplayMessage("Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + objmain.NSEFUTlots + ".", 2);
                                        //  objmain.insertSurveillanceMessages(objtrd.clientcode, "Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + objmain.NSEFUTlots + ".", "");
                                        return false;
                                    }
                                    break;

                                case 3:
                                    if (totLots > exchdata.ncdex)
                                    {
                                        responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + exchdata.ncdex + "";
                                        //  objmain.DisplayMessage("Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + objmain.NCDEXlots + ".", 2);
                                        //  objmain.insertSurveillanceMessages(objtrd.clientcode, "Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + objmain.NCDEXlots + ".", "");
                                        return false;
                                    }
                                    break;

                                case 4:
                                    if (totLots > exchdata.nsecurr)
                                    {
                                        responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + exchdata.nsecurr + "";
                                        //  objmain.DisplayMessage("Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + objmain.NSECURlots + ".", 2);
                                        //  objmain.insertSurveillanceMessages(objtrd.clientcode, "Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + objmain.NSECURlots + ".", "");
                                        return false;
                                    }
                                    break;
                            }
                            break;
                        case 2:
                            switch (objtrd.exch)//Define Exchangewise Lotsize
                            {
                                case 1:
                                    if (totLots > objlimits.mcxlots)
                                    {
                                        responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + objlimits.mcxlots + "";
                                        return false;
                                    }
                                    break;

                                case 2:
                                    if (totLots > objlimits.nsefutlots)
                                    {
                                        responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + objlimits.nsefutlots + ".";
                                        return false;
                                    }
                                    break;

                                case 3:
                                    if (totLots > objlimits.ncxlots)
                                    {
                                        responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + objlimits.ncxlots + ".";
                                        return false;
                                    }
                                    break;

                                case 4:
                                    if (totLots > objlimits.nsecurlots)
                                    {
                                        responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + objlimits.nsecurlots + ".";
                                        return false;
                                    }
                                    break;
                            }
                            break;

                        case 3://Symbolwise Lotwise
                            string[] data = objtrd.Symbol.Split(' ');
                            //SymbolMargin objSymMrgn = objmain.GetSymbolwiseMrgn(objtrd.clientcode, data[0]);
                            if (SymbolLots > objSymMrgn.totlots)
                            {
                                responseMsg = "Exceeding Symbolwise Lotwise Margin, allowed margin for " + objtrd.Symbol + " is " + objSymMrgn.totlots + ".";
                                return false;
                            }
                            break;
                    }
                    break;

                case 3: //Symbolwise
                    if (MarginUtilised > Convert.ToDouble(objlimits.cashmrgn))
                    {
                        responseMsg = "Exceeding Cash Margin Limit, allowed Cash margin Limit is " + objlimits.cashmrgn + ".";
                        return false;
                    }
                    break;

                case 4:
                    if (TotTurnover > Convert.ToDouble(objlimits.turnoverlimit))//Turnover wise
                    {
                        responseMsg = "Turnover exceeding Turnover Margin.";
                        return false;
                    }
                    else
                    {
                        switch (objlimits.lotwisetype)//Lotwise
                        {
                            case 1:
                                DEFAULTEXCHMRGN exchdata = getdata(objinfo.createdby);
                                switch (objtrd.exch)//Default Exchangewise Lotsize
                                {
                                    case 1:
                                        if (totLots > exchdata.mcx)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + exchdata.mcx + "";
                                            return false;
                                            //objmain.DisplayMessage("Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + objmain.MCXlots + ".", 2);
                                            //objmain.insertSurveillanceMessages(objtrd.clientcode, "Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + objmain.MCXlots + ".", "");

                                        }
                                        break;

                                    case 2:
                                        if (totLots > exchdata.nsefut)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + exchdata.nsefut + "";
                                            //  objmain.DisplayMessage("Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + objmain.NSEFUTlots + ".", 2);
                                            //  objmain.insertSurveillanceMessages(objtrd.clientcode, "Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + objmain.NSEFUTlots + ".", "");
                                            return false;
                                        }
                                        break;

                                    case 3:
                                        if (totLots > exchdata.ncdex)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + exchdata.ncdex + "";
                                            //  objmain.DisplayMessage("Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + objmain.NCDEXlots + ".", 2);
                                            //  objmain.insertSurveillanceMessages(objtrd.clientcode, "Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + objmain.NCDEXlots + ".", "");
                                            return false;
                                        }
                                        break;

                                    case 4:
                                        if (totLots > exchdata.nsecurr)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + exchdata.nsecurr + "";
                                            //  objmain.DisplayMessage("Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + objmain.NSECURlots + ".", 2);
                                            //  objmain.insertSurveillanceMessages(objtrd.clientcode, "Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + objmain.NSECURlots + ".", "");
                                            return false;
                                        }
                                        break;
                                }
                                break;
                            case 2:
                                switch (objtrd.exch)//Define Exchangewise Lotsize
                                {

                                    case 1:
                                        if (totLots > objlimits.mcxlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + objlimits.mcxlots + "";
                                            return false;
                                        }
                                        break;

                                    case 2:
                                        if (totLots > objlimits.nsefutlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + objlimits.nsefutlots + ".";
                                            return false;
                                        }
                                        break;

                                    case 3:
                                        if (totLots > objlimits.ncxlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + objlimits.ncxlots + ".";
                                            return false;
                                        }
                                        break;

                                    case 4:
                                        if (totLots > objlimits.nsecurlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + objlimits.nsecurlots + ".";
                                            return false;
                                        }
                                        break;
                                }
                                break;

                            case 3://Symbolwise Lotwise
                                string[] data = objtrd.Symbol.Split(' ');
                                //SymbolMargin objSymMrgn = objmain.GetSymbolwiseMrgn(objtrd.clientcode, data[0]);
                                if (SymbolLots > objSymMrgn.totlots)
                                {
                                    responseMsg = "Exceeding Symbolwise Lotwise Margin, allowed margin for " + objtrd.Symbol + " is " + objSymMrgn.totlots + ".";
                                    return false;
                                }
                                break;


                        }
                        break;
                    }
                case 5:
                    if (MarginUtilised > Convert.ToDouble(objlimits.cashmrgn))
                    {
                        responseMsg = "Exceeding Cash Margin Limit, allowed Cash margin Limit is " + objlimits.cashmrgn + ".";
                        return false;
                    }
                    else
                    {
                        switch (objlimits.lotwisetype)//Lotwise
                        {
                            case 1:

                                break;

                            case 2:
                                switch (objtrd.exch)//Define Exchangewise Lotsize
                                {

                                    case 1:
                                        if (totLots > objlimits.mcxlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + objlimits.mcxlots + "";
                                            return false;
                                        }
                                        break;

                                    case 2:
                                        if (totLots > objlimits.nsefutlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + objlimits.nsefutlots + ".";
                                            return false;
                                        }
                                        break;

                                    case 3:
                                        if (totLots > objlimits.ncxlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + objlimits.ncxlots + ".";
                                            return false;
                                        }
                                        break;

                                    case 4:
                                        if (totLots > objlimits.nsecurlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + objlimits.nsecurlots + ".";
                                            return false;
                                        }
                                        break;
                                }
                                break;

                            case 3://Symbolwise Lotwise
                                string[] data = objtrd.Symbol.Split(' ');
                                //SymbolMargin objSymMrgn = objmain.GetSymbolwiseMrgn(objtrd.clientcode, data[0]);
                                if (SymbolLots > objSymMrgn.totlots)
                                {
                                    responseMsg = "Exceeding Symbolwise Lotwise Margin, allowed margin for " + objtrd.Symbol + " is " + objSymMrgn.totlots + ".";
                                    return false;
                                }
                                break;

                        }
                        break;
                    }
                case 6:
                    if (TotTurnover > Convert.ToDouble(objlimits.turnoverlimit))//Turnover wise
                    {
                        responseMsg = "Turnover exceeding Turnover Margin.";
                        return false;
                    }
                    if (MarginUtilised > Convert.ToDouble(objlimits.cashmrgn))
                    {
                        responseMsg = "Exceeding Cash Margin Limit, allowed Cash margin Limit is " + objlimits.cashmrgn + ".";
                        return false;
                    }
                    break;
            }
            return true;
        }
        public DEFAULTEXCHMRGN getdata(string clientcode)
        {
            using (var cmd = new SqlCommand("[GetExchwiseMarginLots]", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@clientcode", clientcode);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DEFAULTEXCHMRGN _data = new DEFAULTEXCHMRGN();
                        if (!reader.IsDBNull(0))
                            _data.Clientcode = reader.GetString(0);

                        if (!reader.IsDBNull(1))
                            _data.mcx = reader.GetInt32(1);

                        if (!reader.IsDBNull(2))
                            _data.nsefut = reader.GetInt32(2);

                        if (!reader.IsDBNull(3))
                            _data.nsecurr = reader.GetInt32(3);


                        if (!reader.IsDBNull(4))
                            _data.ncdex = reader.GetInt32(4);
                        return _data;
                    }
                }
            }
            return new DEFAULTEXCHMRGN();
        }

      public bool ValidateMarginsqoff(Trades objtrd, double TotTurnover, Limits objlimits, User_info objinfo, int totLots, double MarginUtilised, int SymbolLots, ref string responseMsg, SymbolMargin objSymMrgn)
        {
            switch (objlimits.mrgntype)
            {
                case 1:
                    if (TotTurnover > Convert.ToDouble(objlimits.turnoverlimit))//Turnover wise
                    {
                        responseMsg = "Turnover exceeding Turnover Margin.";
                        return false;
                    }
                    break;
                case 3: //Symbolwise
                    if (MarginUtilised > Convert.ToDouble(objlimits.cashmrgn))
                    {
                        responseMsg = "Exceeding Cash Margin Limit, allowed Cash margin Limit is " + objlimits.cashmrgn + ".";
                        return false;
                    }
                    break;
                case 4:
                    if (TotTurnover > Convert.ToDouble(objlimits.turnoverlimit))//Turnover wise
                    {
                        responseMsg = "Turnover exceeding Turnover Margin.";
                        return false;
                    }
                    else
                    {
                        switch (objlimits.lotwisetype)//Lotwise
                        {
                            case 1:
                                DEFAULTEXCHMRGN exchdata = getdata(objinfo.createdby);
                                switch (objtrd.exch)//Default Exchangewise Lotsize
                                {
                                    case 1:
                                        if (totLots > exchdata.mcx)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + exchdata.mcx + "";
                                            return false;
                                            //objmain.DisplayMessage("Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + objmain.MCXlots + ".", 2);
                                            //objmain.insertSurveillanceMessages(objtrd.clientcode, "Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + objmain.MCXlots + ".", "");

                                        }
                                        break;

                                    case 2:
                                        if (totLots > exchdata.nsefut)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + exchdata.nsefut + "";
                                            //  objmain.DisplayMessage("Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + objmain.NSEFUTlots + ".", 2);
                                            //  objmain.insertSurveillanceMessages(objtrd.clientcode, "Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + objmain.NSEFUTlots + ".", "");
                                            return false;
                                        }
                                        break;

                                    case 3:
                                        if (totLots > exchdata.ncdex)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + exchdata.ncdex + "";
                                            //  objmain.DisplayMessage("Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + objmain.NCDEXlots + ".", 2);
                                            //  objmain.insertSurveillanceMessages(objtrd.clientcode, "Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + objmain.NCDEXlots + ".", "");
                                            return false;
                                        }
                                        break;

                                    case 4:
                                        if (totLots > exchdata.nsecurr)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + exchdata.nsecurr + "";
                                            //  objmain.DisplayMessage("Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + objmain.NSECURlots + ".", 2);
                                            //  objmain.insertSurveillanceMessages(objtrd.clientcode, "Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + objmain.NSECURlots + ".", "");
                                            return false;
                                        }
                                        break;
                                }
                                break;
                            case 2:
                                switch (objtrd.exch)//Define Exchangewise Lotsize
                                {

                                    case 1:
                                        if (totLots > objlimits.mcxlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + objlimits.mcxlots + "";
                                            return false;
                                        }
                                        break;

                                    case 2:
                                        if (totLots > objlimits.nsefutlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + objlimits.nsefutlots + ".";
                                            return false;
                                        }
                                        break;

                                    case 3:
                                        if (totLots > objlimits.ncxlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + objlimits.ncxlots + ".";
                                            return false;
                                        }
                                        break;

                                    case 4:
                                        if (totLots > objlimits.nsecurlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + objlimits.nsecurlots + ".";
                                            return false;
                                        }
                                        break;
                                }
                                break;

                            case 3://Symbolwise Lotwise
                                string[] data = objtrd.Symbol.Split(' ');
                                //SymbolMargin objSymMrgn = objmain.GetSymbolwiseMrgn(objtrd.clientcode, data[0]);
                                if (SymbolLots > objSymMrgn.totlots)
                                {
                                    responseMsg = "Exceeding Symbolwise Lotwise Margin, allowed margin for " + objtrd.Symbol + " is " + objSymMrgn.totlots + ".";
                                    return false;
                                }
                                break;


                        }
                        break;
                    }
                case 5:
                    if (MarginUtilised > Convert.ToDouble(objlimits.cashmrgn))
                    {
                        responseMsg = "Exceeding Cash Margin Limit, allowed Cash margin Limit is " + objlimits.cashmrgn + ".";
                        return false;
                    }
                    else
                    {
                        switch (objlimits.lotwisetype)//Lotwise
                        {
                            case 1:

                                break;

                            case 2:
                                switch (objtrd.exch)//Define Exchangewise Lotsize
                                {

                                    case 1:
                                        if (totLots > objlimits.mcxlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for MCX is " + objlimits.mcxlots + "";
                                            return false;
                                        }
                                        break;

                                    case 2:
                                        if (totLots > objlimits.nsefutlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSEFUT is " + objlimits.nsefutlots + ".";
                                            return false;
                                        }
                                        break;

                                    case 3:
                                        if (totLots > objlimits.ncxlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NCDEX is " + objlimits.ncxlots + ".";
                                            return false;
                                        }
                                        break;

                                    case 4:
                                        if (totLots > objlimits.nsecurlots)
                                        {
                                            responseMsg = "Exceeding Lotwise Margin, allowed lotwise margin for NSECURR is " + objlimits.nsecurlots + ".";
                                            return false;
                                        }
                                        break;
                                }
                                break;

                            case 3://Symbolwise Lotwise
                                string[] data = objtrd.Symbol.Split(' ');
                                //SymbolMargin objSymMrgn = objmain.GetSymbolwiseMrgn(objtrd.clientcode, data[0]);
                                if (SymbolLots > objSymMrgn.totlots)
                                {
                                    responseMsg = "Exceeding Symbolwise Lotwise Margin, allowed margin for " + objtrd.Symbol + " is " + objSymMrgn.totlots + ".";
                                    return false;
                                }
                                break;

                        }
                        break;
                    }
                case 6:
                    if (TotTurnover > Convert.ToDouble(objlimits.turnoverlimit))//Turnover wise
                    {
                        responseMsg = "Turnover exceeding Turnover Margin.";
                        return false;
                    }
                    if (MarginUtilised > Convert.ToDouble(objlimits.cashmrgn))
                    {
                        responseMsg = "Exceeding Cash Margin Limit, allowed Cash margin Limit is " + objlimits.cashmrgn + ".";
                        return false;
                    }
                    break;
            }
            return true;
        }

        public bool PlaceOrder(Trades objord, SqlConnection conn, Feeds objfeeds, ref string msg)
        {
            decimal price = 0;
            if (objord.buysell == 1)
            {
                price = objfeeds.ask;
            }
            else if (objord.buysell == 2)
            {
                price = objfeeds.bid;
            }
            if (price > 0)
            {
                if (objord.Ordprice > 0 && objord.qty > 0)
                {
                    if (objord.ordstatus == 2)
                    {
                        bool ordres = SaveOrder(objord, conn);
                        if (ordres)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (objord.price > 0)
                        {
                            bool ordres = SaveOrder(objord, conn);
                            bool trdres = SaveTrade(objord, conn);
                            if (ordres && trdres)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            else
            {
                msg = "{\"message\":\"fail\", \"description\":\"Unable to Trade during curcuit\"";
                return false;
            }

            return false;
        }
        public void ManageBrokerageType(BrkgType _data)
        {
            if (!_ClientBrokerageType.ContainsKey(_data.clientCode))
                _ClientBrokerageType.Add(_data.clientCode, _data);
            else
                _ClientBrokerageType[_data.clientCode] = _data;
        }
        public  void DownloadBrkgType(string code)
        {
            if (code != null & code != string.Empty)
            {
                using (var cmd = new SqlCommand("getClientBrkgType", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@clientcode", code);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BrkgType _data = new BrkgType();
                            _data.clientCode = reader.GetString(0);
                            _data.mcx = !reader.IsDBNull(1) ? reader.GetInt32(1) : 0;
                            _data.nsefut = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0;
                            _data.ncdex = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0;
                            _data.nsecurr = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0;
                            ManageBrokerageType(_data);
                        }
                    }
                }
            }
        }

        //public string data(test2 data, SqlConnection feedConn)
        //{
        //    int i = 0;
        //    string msg = string.Empty;
        //   // var _data = JsonConvert.DeserializeObject<dynamic>(data.ToString());
        //    using (SqlCommand cmd = new SqlCommand("insert into test2 values(" + data.id + ",'" + data.Surname + "')", feedConn))
        //    {
        //        try
        //        {
        //            i = cmd.ExecuteNonQuery();
        //        }
        //        catch (Exception ex)
        //        {
        //            msg = ex.Message;
        //        }
        //        if (i > 0)
        //        {
        //            msg = "successfull";
        //        }
        //        else
        //        {
        //            msg = "failed  " + msg;
        //        }

        //    }
        //    return msg;
        //}
    }
}