using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ssmarketnewTokenBase.ClassFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace ssmarketnewTokenBase.Controllers
{
    [Authorize]
    public class ReportsController : ApiController
    {
        [Route("api/Reports/getMargin")]
        public HttpResponseMessage getMargin(string clientcode)
        {


            SqlConnection Conn = Utils.conn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            common cm = new common();
            HttpContext httpContext = HttpContext.Current;
            //if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            //{
            //    common.status = "Failed";
            //    common.result = li.Count().ToString() + " record found";
            //    response.description = common;
            //}
            //else
            //{
                var identity = (ClaimsIdentity)User.Identity;
                if (identity.Name != clientcode)
                {
                    cm.Message = "Unauthorized Access for Clientcode= " + clientcode;
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    JSonResponse js1 = new JSonResponse();
                    var object11 = js1.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, object11);
                }
                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();
                }
                using (SqlCommand cmd = new SqlCommand("getMarginForMobile", Conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@clientcode", clientcode);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            margin mar = new margin();
                            if (!reader.IsDBNull(0))
                                mar.cashmrgn = Convert.ToDouble(reader.GetValue(0)) / 100;
                            if (!reader.IsDBNull(1))
                                mar.turnover = Convert.ToDouble(reader.GetValue(1)) / 100;
                            if (!reader.IsDBNull(2))
                                mar.MTMlosslimit = Convert.ToDouble(reader.GetValue(2)) / 100;
                            switch (reader.GetInt32(3))
                            {
                                case 1:
                                    mar.margintype = "Turnover";
                                    break;

                                case 2:
                                    mar.margintype = "Lotwise";
                                    break;

                                case 3:
                                    mar.margintype = "Symbolwise in Rs.";
                                    break;
                                case 4:
                                    mar.margintype = "Turnover + Lotwise";
                                    break;
                                case 5:
                                    mar.margintype = "Lotwise + Symbolwise in Rs.";
                                    break;

                                case 6:
                                    mar.margintype = "Turnover + Symbolwise in Rs.";
                                    break;
                                case 7:
                                    mar.margintype = "Turnover + Lotwise + Symbolwise in Rs.";
                                    break;
                            }
                            li.Add(new margin { cashmrgn = mar.cashmrgn, turnover = mar.turnover, MTMlosslimit = mar.MTMlosslimit, margintype = mar.margintype });
                        }
                    }
                }

                response.data = li;
                common.status = "Success";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            //}
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }
        [Route("api/Reports/getOrders")]
        public HttpResponseMessage getOrders(string clientcode)
        {
            SqlConnection Conn = Utils.conn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            common cm = new common();
            HttpContext httpContext = HttpContext.Current;
            //if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            //{
            //    common.status = "Failed";
            //    common.result = li.Count().ToString() + " record found";
            //    response.description = common;
            //}
            //else
            //{
                var identity = (ClaimsIdentity)User.Identity;
                if (identity.Name != clientcode)
                {
                    cm.Message = "Unauthorized Access for Clientcode= " + clientcode;
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    JSonResponse js1 = new JSonResponse();
                    var object11 = js1.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, object11);
                }
                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();
                }
                using (SqlCommand cmd = new SqlCommand("getOrdersForMobile", Conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@clientcode", clientcode);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Orders mar = new Orders();
                            if (!reader.IsDBNull(0))
                                mar.Symbol = reader.GetString(0);
                            if (!reader.IsDBNull(1))
                                mar.Exch = Utils.GetExch(reader.GetInt32(1));

                            if (reader.GetInt32(2) == 1)
                                mar.BuySell = "BUY";
                            else
                                mar.BuySell = "SELL";
                            if (!reader.IsDBNull(3))
                                mar.Qty = reader.GetInt32(3);
                            if (!reader.IsDBNull(4))
                                mar.OrdPrice = Convert.ToDouble(reader.GetValue(4));
                            if (!reader.IsDBNull(5))
                                mar.Orderno = reader.GetValue(5).ToString();
                            if (reader.GetInt32(6) == 1)
                                mar.Validity = "CNF";
                            else
                                mar.Validity = "DAY";
                            if (!reader.IsDBNull(7))
                                mar.CreateOn = reader.GetValue(7).ToString();
                            switch (reader.GetInt32(8))
                            {
                                case 1:
                                    mar.OrdStatus = "EXECUTED";
                                    break;

                                case 2:
                                    mar.OrdStatus = "PENDING";
                                    break;

                                case 3:
                                    mar.OrdStatus = "CANCELLED";
                                    break;
                            }

                            if (reader.GetInt32(9) == 2)
                                mar.Productype = "SL";
                            else
                                mar.Productype = "RL";
                            if (!reader.IsDBNull(10))
                                mar.Price = Convert.ToDouble(reader.GetValue(10));


                            li.Add(new Orders { Symbol = mar.Symbol, Exch = mar.Exch, BuySell = mar.BuySell, Qty = mar.Qty, OrdPrice = mar.OrdPrice, Orderno = mar.Orderno, Validity = mar.Validity, CreateOn = mar.CreateOn, OrdStatus = mar.OrdStatus, Productype = mar.Productype, Price = mar.Price });
                        }
                    }
                }

                response.data = li;
                common.status = "Success";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            //}
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }
        [Route("api/Reports/getTrades")]
        public HttpResponseMessage getTrades(string clientcode, string fromdate, string todate)
        {
            SqlConnection Conn = Utils.conn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            common cm = new common();
            HttpContext httpContext = HttpContext.Current;
            //if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            //{
            //    common.status = "Failed";
            //    common.result = li.Count().ToString() + " record found";
            //    response.description = common;
            //}
            //else
            //{
                //var identity = (ClaimsIdentity)User.Identity;
                //if (identity.Name != clientcode)
                //{
                //    cm.Message = "Unauthorized Access for Clientcode= " + clientcode;
                //    li.Add(new common { Message = cm.Message });
                //    response.data = li;
                //    JSonResponse js1 = new JSonResponse();
                //    var object11 = js1.JSon(response);
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, object11);
                //}
                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();
                }

                DateTime _fromdate = Convert.ToDateTime(fromdate);
                DateTime _todate = Convert.ToDateTime(todate);
                if (_fromdate.Day == DateTime.Today.Day && _fromdate.Month == DateTime.Today.Month && _fromdate.Year == DateTime.Today.Year)
                {
                    using (SqlCommand cmd = new SqlCommand("getTradeForMobile", Conn) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@clientcode", clientcode);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Trade mar = new Trade();
                                if (!reader.IsDBNull(0))
                                    mar.Symbol = reader.GetString(0);
                                if (!reader.IsDBNull(1))
                                    mar.Exch = Utils.GetExch(reader.GetInt32(1));

                                if (reader.GetInt32(2) == 1)
                                    mar.BuySell = "BUY";
                                else
                                    mar.BuySell = "SELL";
                                if (!reader.IsDBNull(3))
                                    mar.Qty = reader.GetInt32(3);
                                if (!reader.IsDBNull(4))
                                    mar.OrdPrice = Convert.ToDouble(reader.GetValue(4));
                                if (!reader.IsDBNull(5))
                                    mar.Orderno = reader.GetValue(5).ToString();
                                if (reader.GetInt32(6) == 1)
                                    mar.Validity = "CNF";
                                else
                                    mar.Validity = "DAY";
                                if (!reader.IsDBNull(7))
                                    mar.CreateOn = reader.GetValue(7).ToString();
                                if (!reader.IsDBNull(8))
                                    mar.Price = Convert.ToDouble(reader.GetValue(8));


                                li.Add(new Trade { Symbol = mar.Symbol, Exch = mar.Exch, BuySell = mar.BuySell, Qty = mar.Qty, OrdPrice = mar.OrdPrice, Orderno = mar.Orderno, Validity = mar.Validity, CreateOn = mar.CreateOn, Price = mar.Price, status = "Executed" });
                            }
                        }
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand("GetTradeHistory", Conn) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@clientcode", clientcode);
                        cmd.Parameters.AddWithValue("@fromtime", _fromdate.ToString("yyyy-MM-dd 09:00:30"));
                        cmd.Parameters.AddWithValue("@todate", _todate.ToString("yyyy-MM-dd 23:59:00"));
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Trade mar = new Trade();
                                if (!reader.IsDBNull(0))
                                    mar.Symbol = reader.GetString(0);
                                if (!reader.IsDBNull(1))
                                    mar.Exch = reader.GetString(1);
                                if (!reader.IsDBNull(2))
                                    mar.BuySell = reader.GetString(2);

                                if (!reader.IsDBNull(3))
                                    mar.Qty = reader.GetInt32(3);
                                if (!reader.IsDBNull(4))
                                    mar.OrdPrice = Convert.ToDouble(reader.GetValue(4));
                                if (!reader.IsDBNull(5))
                                    mar.Orderno = reader.GetValue(5).ToString();
                                if (!reader.IsDBNull(6))
                                    mar.Validity = reader.GetString(6);
                                if (!reader.IsDBNull(7))
                                    mar.CreateOn = reader.GetValue(7).ToString();                                                                                                                                                                                                                                                                                         
                                if (!reader.IsDBNull(8))
                                    mar.Price = Convert.ToDouble(reader.GetValue(8));


                                li.Add(new Trade { Symbol = mar.Symbol, Exch = mar.Exch, BuySell = mar.BuySell, Qty = mar.Qty, OrdPrice = mar.OrdPrice, Orderno = mar.Orderno, Validity = mar.Validity, CreateOn = mar.CreateOn, Price = mar.Price, status = "Executed" });


                            }
                        }
                    }

                }
                response.data = li;
                common.status = "Success";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            //}
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }
        [Route("api/Reports/GetProfitloss")]
        public HttpResponseMessage GetProfitloss(string clientcode, string startdate, string enddate)
        {
            Utils _utils = new Utils();
            SqlConnection feedConn = Utils.feedconn;
            SqlConnection Conn = Utils.conn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            common cm = new common();
            HttpContext httpContext = HttpContext.Current;
            //if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            //{
            //    common.status = "Failed";
            //    common.result = li.Count().ToString() + " record found";
            //    response.description = common;
            //}
            //else
            //{
                var identity = (ClaimsIdentity)User.Identity;
                if (identity.Name != clientcode)
                {
                    //li.Add("Unauthorized Access for Clientcode= " + clientcode);
                    cm.Message = "Unauthorized Access for Clientcode= " + clientcode;
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    JSonResponse js1 = new JSonResponse();
                    var object11 = js1.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, object11);
                }
                if (feedConn.State != ConnectionState.Open)
                {
                    feedConn.Open();
                }
                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();
                }
                _utils.DownloadBrkgType(clientcode);
                _utils.GetLimits(clientcode, Conn);
                User_info objuserinfo = _utils.GetUserinfo(clientcode, Conn);

                DateTime fromdate = DateTime.Now;
                DateTime todate = DateTime.Now;
                fromdate = Convert.ToDateTime(startdate);
                todate = Convert.ToDateTime(enddate);
                response _res = new response();
                string time = DateTime.Now.ToString("yyyy-MM-dd");
                if (startdate != time)
                {

                    if (Conn.State == ConnectionState.Open)
                    {
                        using (SqlCommand cmd = new SqlCommand("getprofitlossformobile", Conn) { CommandType = CommandType.StoredProcedure })
                        {
                            cmd.Parameters.AddWithValue("@clientcode", clientcode);
                            cmd.Parameters.AddWithValue("@fromdate", fromdate.ToString("yyyy-MM-dd 00:mm:ss"));
                            cmd.Parameters.AddWithValue("@todate", todate.ToString("yyyy-MM-dd HH:mm:ss"));
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (!reader.IsDBNull(0))
                                        _res.ClientCode = reader.GetString(0);
                                    if (!reader.IsDBNull(1))
                                        _res.Exch = reader.GetString(1);
                                    if (!reader.IsDBNull(2))
                                        _res.Symbol = reader.GetString(2);
                                    _utils.GetSymbolwiseMrgn(objuserinfo, _res.Symbol, Conn);
                                    _utils.GetContract(_res.Symbol, Conn);
                                    if (!reader.IsDBNull(3))
                                        _res.Validity = reader.GetString(3);
                                    if (!reader.IsDBNull(4))
                                        _res.Lotsize = reader.GetInt32(4).ToString();
                                    if (!reader.IsDBNull(5))
                                        _res.Bqty = reader.GetInt32(5).ToString();
                                    if (!reader.IsDBNull(6))
                                        _res.BuyPrice = reader.GetValue(6).ToString();
                                    if (!reader.IsDBNull(7))
                                        _res.Sqty = reader.GetInt32(7).ToString();
                                    if (!reader.IsDBNull(8))
                                        _res.SellPrice = reader.GetValue(8).ToString();
                                    if (!reader.IsDBNull(9))
                                        _res.Net = reader.GetInt32(9).ToString();
                                    if (!reader.IsDBNull(10))
                                        _res.Cmp = reader.GetValue(10).ToString();
                                    if (!reader.IsDBNull(11))
                                        _res.RealisedPL = reader.GetValue(11).ToString();
                                    if (!reader.IsDBNull(13))
                                        _res.Brokerage = reader.GetValue(13).ToString();
                                    if (!reader.IsDBNull(12))
                                        _res.UnrealisedPL = reader.GetValue(12).ToString();
                                    if (!reader.IsDBNull(14))
                                        _res.NetMTMPL = reader.GetValue(14).ToString();
                                    if (!reader.IsDBNull(15))
                                        _res.Time = reader.GetValue(15).ToString();
                                    li.Add(new response
                                    {
                                        ClientCode = _res.ClientCode,
                                        Exch = _res.Exch,
                                        Symbol = _res.Symbol,
                                        Validity = _res.Validity,
                                        Lotsize = _res.Lotsize,
                                        Bqty = _res.Bqty,
                                        BuyPrice = _res.BuyPrice,
                                        Sqty = _res.Sqty,
                                        SellPrice = _res.SellPrice,
                                        Net = _res.Net,
                                        Cmp = _res.Cmp,
                                        RealisedPL = _res.RealisedPL,
                                        Brokerage = _res.Brokerage,
                                        NetMTMPL = _res.NetMTMPL,
                                        Time = _res.Time
                                    });

                                }
                            }
                        }
                    }

                }
                else
                {
                    //DefaultData(feedconn);
                    Dictionary<string, buysellPos> _BuySellAvgPos = new Dictionary<string, buysellPos>();
                    _utils.GetTradePosition(ref _BuySellAvgPos, clientcode, false);
                    Dictionary<string, buysellnetpospfls> _NetPosPfLs = _utils.ProcessProfitLoss1(_BuySellAvgPos, Conn, clientcode);
                    Dictionary<string, buysellnetpospfls> _PosPfLs = new Dictionary<string, buysellnetpospfls>(_NetPosPfLs);
                    // DefaultData(feedconn, _NetPosPfLs);
                    //response _res = new response();
                    //List<response> resp = new List<response>();
                    foreach (var items in _NetPosPfLs)
                    {
                        string[] data = items.Key.Split('_');
                        string symbol = data[0];
                        string account = data[1];
                        int validity = Convert.ToInt32(data[2]);
                        //if (_Symconctracts.ContainsKey(symbol))
                        //{
                        Contracts objcon = new Contracts();
                        objcon = _utils.GetContract(symbol, Conn);
                        buysellnetpospfls objpos = items.Value;
                        _res.ClientCode = clientcode;
                        _res.Exch = Utils.GetExch(objcon.exch);
                        _res.Symbol = symbol;
                        _res.Validity = _utils.GetStringValidity(validity);
                        _res.Lotsize = objcon.lotsize.ToString();
                        _res.Bqty = objpos.BQty.ToString();
                        _res.BuyPrice = Decimal.Round(objpos.buyprice, 2).ToString();
                        _res.Sqty = objpos.SQty.ToString();
                        _res.SellPrice = Decimal.Round(objpos.sellprice, 2).ToString();
                        _res.Net = (objpos.BQty - objpos.SQty).ToString();
                        string feedsymbol = objcon.SymDesp;
                        Feeds objfeeds = _utils.GetDBFeed(symbol, feedConn);
                        // Feeds objfeeds = Feed(feedsymbol);
                        if (objpos.BQty > objpos.SQty)
                        {
                            //int netqty = objpos.BQty - objpos.SQty;
                            //decimal mktPL = (objfeeds.bid - Decimal.Round(objpos.buyprice, 2)) * netqty * objcon.lotsize;
                            decimal NETPL = Decimal.Round(objpos.p_l, 2) + objpos.UnrealisedP_l - objpos.Commision;
                            _res.Cmp = objfeeds.bid.ToString();
                            _res.UnrealisedPL = Decimal.Round(objpos.UnrealisedP_l, 2).ToString();
                            _res.NetMTMPL = Decimal.Round(NETPL, 2).ToString();
                        }
                        else
                        {
                            //int netqty = objpos.SQty - objpos.BQty;
                            //decimal mktPL = (Decimal.Round(objpos.sellprice, 2) - objfeeds.ask) * netqty * objcon.lotsize;
                            decimal NETPL = Decimal.Round(objpos.p_l, 2) + objpos.UnrealisedP_l - objpos.Commision;
                            _res.Cmp = objfeeds.ask.ToString();
                            _res.UnrealisedPL = Decimal.Round(objpos.UnrealisedP_l, 2).ToString();
                            _res.NetMTMPL = Decimal.Round(NETPL, 2).ToString();
                        }
                        _res.RealisedPL = Decimal.Round(objpos.p_l, 2).ToString();
                        _res.Brokerage = objpos.Commision.ToString();

                        li.Add(new response
                        {
                            ClientCode = _res.ClientCode,
                            Exch = _res.Exch,
                            Symbol = _res.Symbol,
                            Validity = _res.Validity,
                            Lotsize = _res.Lotsize,
                            Bqty = _res.Bqty,
                            BuyPrice = _res.BuyPrice,
                            Sqty = _res.Sqty,
                            SellPrice = _res.SellPrice,
                            Net = _res.Net,
                            Cmp = _res.Cmp,
                            RealisedPL = _res.RealisedPL,
                            UnrealisedPL = _res.UnrealisedPL,
                            Brokerage = _res.Brokerage,
                            NetMTMPL = _res.NetMTMPL
                        });

                        // }

                    }


                }
                response.data = li;
                common.status = "Success";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            //}
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);

            // return Request.CreateResponse(HttpStatusCode.OK, resp);

        }

        [Route("api/Reports/Getledger")]
        public HttpResponseMessage Getledger(string clientcode, string start, string end)//2021-10-18 09:00:00 2021-10-23 23:59:00
        {
            Utils _utils = new Utils();
            SqlConnection feedConn = Utils.feedconn;
            SqlConnection Conn = Utils.conn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            HttpContext httpContext = HttpContext.Current;
            //if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            //{
            //    common.status = "Failed";
            //    common.result = li.Count().ToString() + " record found";
            //    response.description = common;
            //}
            //else
            //{

                common cm = new common();
                var identity = (ClaimsIdentity)User.Identity;
                if (identity.Name != clientcode)
                {
                    //li.Add("Unauthorized Access for Clientcode= " + clientcode);
                    cm.Message = "Unauthorized Access for Clientcode= " + clientcode;
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    JSonResponse js1 = new JSonResponse();
                    var object11 = js1.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, object11);
                }
                if (feedConn.State != ConnectionState.Open)
                {
                    feedConn.Open();
                }
                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();
                }
                _utils.DownloadBrkgType(clientcode);
                _utils.GetLimits(clientcode, Conn);
                User_info objuserinfo = _utils.GetUserinfo(clientcode, Conn);
                //DownloadSymbolMargin(clientcode);
                //DownloadBrkgType(clientcode);
                //DownloadLimitMargin(clientcode);
                //DownloadContracts(clientcode);
                start = start + " 00:00:00";
                end = end + " 23:59:00";
                DateTime Fromdate = Convert.ToDateTime(start);
                string payoutstartdate = Fromdate.ToString("yyyy-MM-dd 00:00:00");
                int d = (int)Fromdate.DayOfWeek;
                if (d == 0)
                    payoutstartdate = Fromdate.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                else if (d == 1)
                    payoutstartdate = Fromdate.AddDays(-2).ToString("yyyy-MM-dd 00:00:00");


                Dictionary<string, Ledger> _Ledgervalues = _utils.Ledger(clientcode, start, end);
                if (_Ledgervalues.Count == 0)
                {
                    response.data = li;
                    common.status = "Success";
                    common.result = li.Count().ToString() + " record found";
                    response.description = common;
                    JSonResponse js1 = new JSonResponse();
                    var object11 = js1.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.OK, object11);
                }

                SortedDictionary<DateTime, payinoutDetails> _PayinOutDetails = _utils.GetPayinPayout(clientcode, payoutstartdate, end);

                decimal begbal = _utils.GetBegningBal(clientcode, Fromdate, end);

                decimal comm = 0, pl = 0, netpl = 0;
                double payinout = 0;
                AccountLedger _accledger = new AccountLedger();
                List<AccountLedger> _outputledger = new List<AccountLedger>();
                AccountSummery acc = new AccountSummery();
                Total _total = new Total();
                if (_Ledgervalues.Count > 0)
                {
                    foreach (var items in _Ledgervalues)
                    {
                        Ledger objledger = items.Value;
                        _accledger.ClientCode = objledger.accountno;
                        _accledger.ClosedTime = objledger.CloseTime.ToString("dd/MM/yyyy HH:mm:ss");
                        _accledger.Type = "L";
                        _accledger.Ticket = objledger.orderno.ToString();
                        _accledger.Symbol = objledger.symbol;
                        _accledger.Ammount = objledger.sellqty;
                        if (objledger.OpenTime > objledger.CloseTime)
                        {
                            _accledger.buy_sell = "S";
                            _accledger.Open_Sl = objledger.sellprice.ToString();
                            _accledger.Close_Tp = objledger.buyprice.ToString();
                        }
                        else
                        {
                            _accledger.buy_sell = "B";
                            _accledger.Open_Sl = objledger.buyprice.ToString();
                            _accledger.Close_Tp = objledger.sellprice.ToString();
                        }
                        _accledger.OpenTime = objledger.OpenTime.ToString("dd/MM/yyyy HH:mm:ss");
                        _accledger.Comm = objledger.comm.ToString();
                        _accledger.Profit_Desc = objledger.profitLoss.ToString();
                        _accledger.Net_Profit = objledger.NettPl.ToString();

                        comm += objledger.comm;
                        pl += Convert.ToDecimal(objledger.profitLoss);
                        netpl += objledger.NettPl;

                        _outputledger.Add(new AccountLedger
                        {
                            ClientCode = _accledger.ClientCode,
                            ClosedTime = _accledger.ClosedTime,
                            Type = _accledger.Type,
                            Ticket = _accledger.Ticket,
                            Symbol = _accledger.Symbol,
                            Ammount = _accledger.Ammount,
                            buy_sell = _accledger.buy_sell,
                            OpenTime = _accledger.OpenTime,
                            Open_Sl = _accledger.Open_Sl,
                            Close_Tp = _accledger.Close_Tp,
                            Dip_Widrow = _accledger.Dip_Widrow,
                            Comm = _accledger.Comm,
                            Profit_Desc = _accledger.Profit_Desc,
                            Net_Profit = _accledger.Net_Profit,
                        });
                    }
                    foreach (var itemss in _PayinOutDetails)
                    {
                        _accledger.ClientCode = clientcode;
                        _accledger.ClosedTime = itemss.Key.ToString("dd/MM/yyyy HH:mm:ss");
                        _accledger.Ticket = itemss.Value.comments;
                        if (itemss.Value.payinout.ToUpper() == "PAYIN")
                        {
                            _accledger.Type = "DP";
                            _accledger.Dip_Widrow = itemss.Value.amount.ToString();
                            if (itemss.Value.comments.ToUpper() != "INITIAL MARGIN")
                                payinout += itemss.Value.amount;
                        }
                        else if (itemss.Value.payinout.ToUpper() == "PAYOUT")
                        {
                            _accledger.Type = "WD";
                            _accledger.Dip_Widrow = (itemss.Value.amount * -1).ToString();
                            if (itemss.Value.comments.ToUpper() != "INITIAL MARGIN")
                                payinout -= itemss.Value.amount;
                        }

                        _outputledger.Add(new AccountLedger
                        {
                            ClientCode = _accledger.ClientCode,
                            ClosedTime = _accledger.ClosedTime,
                            Type = _accledger.Type,
                            Ticket = _accledger.Ticket,
                            // Symbol = _accledger.Symbol,
                            // Ammount = _accledger.Ammount,
                            // buy_sell = _accledger.buy_sell,
                            // OpenTime = _accledger.OpenTime,
                            // Open_Sl = _accledger.Open_Sl,
                            // Close_Tp = _accledger.Close_Tp,
                            Dip_Widrow = _accledger.Dip_Widrow,
                            //Comm = _accledger.Comm,
                            // Profit_Desc = _accledger.Profit_Desc,
                            // Net_Profit = _accledger.Net_Profit,
                        });
                    }

                    _total.Totals = "Totals:-";
                    _total.payinout = payinout.ToString();
                    _total.Commition = (comm * -1).ToString();
                    _total.Profit_Loss = pl.ToString();
                    _total.Net_Profit_Loss = netpl.ToString();

                    acc.beg_balance = begbal.ToString();
                    acc.MarginINOUT = payinout.ToString();
                    acc.TradingPL = pl.ToString();
                    acc.Comm = (comm * -1).ToString();
                    acc.Net_TradingPL = netpl.ToString();
                    acc.Balance = (begbal + netpl + Convert.ToDecimal(payinout)).ToString();


                }
            
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new DictionaryAsArrayResolver();
            var json1 = JsonConvert.SerializeObject(_outputledger, settings);
            var json2 = JsonConvert.SerializeObject(_total, settings);
            var json3 = JsonConvert.SerializeObject(acc, settings);
            json1 = "{\"data\":" + json1 + ",\"Total\":" + json2 + ",\"Account Summery\":" + json3 + "}";
            var objects1 = JObject.Parse(json1);
            return Request.CreateResponse(HttpStatusCode.OK, objects1);
       // }
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }

        [Route("api/Reports/GetIntraDayHistory")]
        public HttpResponseMessage GetIntraDayHistory(string symbol, string period)
        {
            Utils _utils = new Utils();
            SqlConnection feedConn = Utils.feedconn;
            SqlConnection Conn = Utils.conn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            common cm = new common();
            HttpContext httpContext = HttpContext.Current;
            //if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            //{
            //    common.status = "Failed";
            //    common.result = li.Count().ToString() + " record found";
            //    response.description = common;
            //}
            //else
            //{
                var identity = (ClaimsIdentity)User.Identity;
            //if (identity.Name != clientcode)
            //{
            //    //li.Add("Unauthorized Access for Clientcode= " + clientcode);
            //    cm.Message = "Unauthorized Access for Clientcode= " + clientcode;
            //    li.Add(new common { Message = cm.Message });
            //    response.data = li;
            //    JSonResponse js1 = new JSonResponse();
            //    var object11 = js1.JSon(response);
            //    return Request.CreateResponse(HttpStatusCode.Unauthorized, object11);
            //}
            if (feedConn.State != ConnectionState.Open)
            {
                feedConn.Open();
            }
            if (Conn.State != ConnectionState.Open)
            {
                Conn.Open();
            }
            // string connstring2 = _utils.Decryptdata(Utils.oldOhlc);
            SqlConnection conn2 = Utils.oldohlcconn;
            try
            {
                if (conn2.State != ConnectionState.Open)
                {
                    conn2.Open();
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, ex.Message); ;
            }
            string exchsymbol = symbol;
            _utils._OHLCBars = new SortedDictionary<DateTime, OHLC>();
            DateTime servertime = _utils.ServerTime();
            // if (_Symconctracts.ContainsKey(exchsymbol))
            // {
            List<exchangeDetails> usExch = _utils.exchange(symbol);
            //Contracts objcon1 = new Contracts();
            //objcon1 = _Symconctracts[exchsymbol];
            int _exch = 0; string symdes = string.Empty;
            foreach (var item in usExch)
            {
                _exch = item.exch;
                symdes = item.symboldes;
            }
            string Symbol = symdes;
            //  string tablename = _utils.Tablename(_exch);
            int periodicity = _utils.Periodicity(period);
            //OHLC _oh = new OHLC();
            using (SqlCommand tickcmd = new SqlCommand(String.Format("getintradaytickdata"), conn2) { CommandType = CommandType.StoredProcedure })
            {
                tickcmd.Parameters.AddWithValue("@tableid", _exch);
                tickcmd.Parameters.AddWithValue("@usersumbol", Symbol);
                tickcmd.Parameters.AddWithValue("@time", servertime.ToString("yyyy-MM-dd 09:00:00"));
                using (SqlDataReader reader = tickcmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string symboll = string.Empty;
                        decimal open = 0, high = 0, low = 0, close = 0;
                        DateTime Time = DateTime.Now;
                        int vol = 0;

                        if (!reader.IsDBNull(0))
                            symboll = reader.GetString(0);
                        else
                            continue;

                        if (!reader.IsDBNull(1))
                            open = Convert.ToDecimal(reader.GetValue(1));

                        if (!reader.IsDBNull(2))
                            high = Convert.ToDecimal(reader.GetValue(2));

                        if (!reader.IsDBNull(3))
                            low = Convert.ToDecimal(reader.GetValue(3));

                        if (!reader.IsDBNull(4))
                            close = Convert.ToDecimal(reader.GetValue(4));

                        if (!reader.IsDBNull(5))
                            Time = Convert.ToDateTime(reader.GetValue(5));

                        if (!reader.IsDBNull(6))
                            vol = Convert.ToInt32(reader.GetValue(6));
                        if (periodicity == 1)
                        {
                            OHLC _oh = new OHLC();
                            _oh.symbol = exchsymbol;
                            _oh.Open = open;
                            _oh.High = high;
                            _oh.Low = low;
                            _oh.CLose = close;
                            _oh.volume = vol;
                            _oh.FeedTime = Time;
                            _utils._OHLCBars.Add(_oh.FeedTime, _oh);

                        }
                        else
                        {
                            DateTime Dt1 = Time;
                            DateTime Dt3 = DateTime.Now;
                            if (Dt1.Minute % periodicity != 0)
                            {
                                int leftmin = (Dt1.Minute % periodicity);
                                Dt3 = Dt1.AddMinutes(leftmin * -1);
                            }
                            else
                            {
                                Dt3 = Dt1;
                            }

                            if (!_utils._OHLCBars.ContainsKey(Dt3))
                            {
                                OHLC objohlc = new OHLC();
                                objohlc.Open = open;
                                objohlc.High = high;
                                objohlc.Low = low;
                                objohlc.CLose = close;
                                objohlc.volume = vol;
                                _utils._OHLCBars.Add(Dt3, objohlc);
                            }
                            else
                            {
                                OHLC objohlc = _utils._OHLCBars[Dt3];
                                if (high > objohlc.High)
                                    objohlc.High = high;

                                if (low < objohlc.Low)
                                    objohlc.Low = low;
                                objohlc.CLose = close;
                                objohlc.volume += vol;
                                _utils._OHLCBars[Dt3] = objohlc;
                            }

                        }
                    }
                }
            }
            // }
                JsonSerializerSettings settings0 = new JsonSerializerSettings();
                settings0.Formatting = Formatting.Indented;
                settings0.ContractResolver = new DictionaryAsArrayResolver();
                var json = JsonConvert.SerializeObject(_utils._OHLCBars, settings0);
                json = "{\"data\":" + json + "}";
                var objects = Newtonsoft.Json.Linq.JObject.Parse(json);
                return Request.CreateResponse(HttpStatusCode.OK, objects);
            //}
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }

        [Route("api/Reports/getCashBalance")]
        public HttpResponseMessage getCashBalance(string clientcode)
        {
            SqlConnection Conn = Utils.conn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            common cm = new common();
            HttpContext httpContext = HttpContext.Current;
            //if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            //{
            //    common.status = "Failed";
            //    common.result = li.Count().ToString() + " record found";
            //    response.description = common;
            //}
            //else
            //{
                Utils _utils = new Utils();
                var identity = (ClaimsIdentity)User.Identity;
                if (identity.Name != clientcode)
                {
                    cm.Message = "Unauthorized Access for Clientcode= " + clientcode;
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    JSonResponse js1 = new JSonResponse();
                    var object11 = js1.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, object11);
                }
                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();
                }
                Limits objlimits = _utils.GetLimits(clientcode, Conn);
                Dictionary<string, buysellPos> _BuySellAvgPos = new Dictionary<string, buysellPos>();
                Dictionary<string, buysellPos> _BuySellAvgPosRMS = new Dictionary<string, buysellPos>();
                Dictionary<string, Dictionary<int, List<Trades>>> FIFOPos = _utils.GetTradePosition(ref _BuySellAvgPos, clientcode, false);
                var _NetProftLoss = _utils.ProcessProfitLoss1(_BuySellAvgPos, Conn, clientcode);
                var data = _utils.GetNetPrfLoss(_NetProftLoss);
                Dictionary<string, buysellnetpospfls> _NetPosPfLs = new Dictionary<string, buysellnetpospfls>(data);
                buysellnetpospfls objpos = new buysellnetpospfls();
                if (_NetPosPfLs.ContainsKey(clientcode))
                {
                    objpos = _NetPosPfLs[clientcode];
                }
                else
                {
                    cm.Message = "No Data Available for " + clientcode;
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    JSonResponse js1 = new JSonResponse();
                    var object11 = js1.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, object11);
                }

                var netMTM = Decimal.Round((objpos.p_l + objpos.UnrealisedP_l) - objpos.p_ltax - objpos.Commision - objpos.Comm_Tax, 2);
                li.Add(new cashbal { clientcode = clientcode, cash_bal = objlimits.cashmrgn, MTMprofitloss = netMTM, AvailableBal = objlimits.cashmrgn + netMTM });
                response.data = li;
                common.status = "Success";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            //}
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }

        [Route("api/Reports/GetDeletedTrade")]
        public HttpResponseMessage GetDeletedTrade(string clientcode, string start, string end)
        {
            SqlConnection Conn = Utils.conn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            common cm = new common();
            HttpContext httpContext = HttpContext.Current;
            //if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            //{
            //    common.status = "Failed";
            //    common.result = li.Count().ToString() + " record found";
            //    response.description = common;
            //}
            //else
            //{
                Utils _utils = new Utils();
                var identity = (ClaimsIdentity)User.Identity;
                if (identity.Name != clientcode)
                {
                    cm.Message = "Unauthorized Access for Clientcode= " + clientcode;
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    JSonResponse js1 = new JSonResponse();
                    var object11 = js1.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, object11);
                }
                if (Conn.State == ConnectionState.Closed)
                    Conn.Open();
                using (var savecmd = new SqlCommand("getmisDeletedTrade", Conn) { CommandType = CommandType.StoredProcedure })
                {
                    savecmd.Parameters.AddWithValue("@clientcode", clientcode);
                    savecmd.Parameters.AddWithValue("@fromdate", start);
                    savecmd.Parameters.AddWithValue("@todate", end);
                    //savecmd.Parameters.AddWithValue("@flag", body.flag);

                    using (var reader = savecmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            deleteTrade rl = new deleteTrade();
                            DateTime dt = DateTime.Now;
                            DateTime dt1 = DateTime.Now;
                            if (!reader.IsDBNull(0))
                                rl.Clientcode = reader.GetString(0);
                            if (!reader.IsDBNull(1))
                                rl.Symbol = reader.GetString(1);
                            if (!reader.IsDBNull(2))
                                rl.Qty = Convert.ToInt32(reader.GetValue(2));
                            if (!reader.IsDBNull(3))
                                rl.Orderno = Convert.ToInt64(reader.GetValue(3));
                            if (!reader.IsDBNull(4))
                                rl.ExecPrice = Convert.ToDecimal(reader.GetValue(4));
                            if (!reader.IsDBNull(5))
                                dt = Convert.ToDateTime(reader.GetValue(5));
                            rl.Timestamp = dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            if (!reader.IsDBNull(6))
                                rl.Buy_Sell = reader.GetString(6);
                            if (!reader.IsDBNull(7))
                                rl.Exchange = reader.GetString(7);
                            if (!reader.IsDBNull(8))
                                rl.Ipaddress = reader.GetString(8);

                            li.Add(new deleteTrade { Symbol = rl.Symbol, Exchange = rl.Exchange, Buy_Sell = rl.Buy_Sell, Qty = rl.Qty, Orderno = rl.Orderno,ExecPrice = rl.ExecPrice,Clientcode=rl.Clientcode,Ipaddress=rl.Ipaddress,Timestamp=rl.Timestamp });
                        }
                    }
                }
               
                response.data = li;
                common.status = "Success";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            //}
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);

        }
    }

    
    internal class cashbal
    {
        public string clientcode { get; set; }
        public decimal cash_bal { get; set; }
        public decimal MTMprofitloss { get; set; }
        public decimal AvailableBal { get; set; }
    }
}



