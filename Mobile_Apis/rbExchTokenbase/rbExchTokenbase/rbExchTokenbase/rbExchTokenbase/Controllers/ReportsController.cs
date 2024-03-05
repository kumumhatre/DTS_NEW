using rbExchTokenbase.ClassFiles;
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

namespace rbExchTokenbase.Controllers
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
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                common.status = "Failed";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            }
            else
            {
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
            }
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
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                common.status = "Failed";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            }
            else
            {
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
            }
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
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                common.status = "Failed";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            }
            else
            {
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
            }
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
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                common.status = "Failed";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            }
            else
            {
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
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);

            // return Request.CreateResponse(HttpStatusCode.OK, resp);
        }
    }
}
