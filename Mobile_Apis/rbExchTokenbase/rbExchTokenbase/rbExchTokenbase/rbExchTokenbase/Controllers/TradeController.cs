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
    public class TradeController : ApiController
    {
        [Route("api/Trade/getPortfolio")]
        public HttpResponseMessage getPortfolio(string clientcode)
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
                    cm.Message = "Unauthorized Access for Clientcode= " + clientcode;
                    li.Add(new common { Message = cm.Message });
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
                Dictionary<string, buysellPos> _BuySellAvgPos = new Dictionary<string, buysellPos>();
                _utils.GetTradePosition(ref _BuySellAvgPos, clientcode, false);
                Dictionary<string, buysellnetpospfls> _NetPosPfLs = _utils.ProcessProfitLoss1(_BuySellAvgPos, Conn, clientcode);
                string portfolio = string.Empty;

                foreach (var items in _NetPosPfLs)
                {
                    string[] data = items.Key.Split('_');
                    string symbol = data[0];
                    string account = data[1];
                    int validity = Convert.ToInt32(data[2]);
                    Contracts objcon = _utils.GetContract(symbol, Conn);
                    buysellnetpospfls objpos = items.Value;
                    Feeds objfeed = _utils.GetDBFeed(symbol, feedConn);
                    string stringvalidity = "CNF";
                    if (validity == 2)
                        stringvalidity = "DAY";
                    if (objpos.buy_sell > 0)
                    {
                        string buysell = "BUY";
                        decimal price = Decimal.Round(objpos.buyprice, 2);
                        decimal pl = 0;
                        if (objpos.buy_sell == 1)
                        {
                            if (objcon.exch == 2)
                                pl = Decimal.Round((objfeed.ltp - objpos.buyprice) * objpos.Qty);
                            else
                                pl = Decimal.Round((objfeed.ltp - objpos.buyprice) * objpos.Qty * objcon.lotsize);
                        }
                        if (objpos.buy_sell == 2)
                        {
                            buysell = "SELL";
                            price = Decimal.Round(objpos.sellprice, 2);
                            if (objcon.exch == 2)
                                pl = Decimal.Round((objpos.sellprice - objfeed.ltp) * objpos.Qty);
                            else
                                pl = Decimal.Round((objpos.sellprice - objfeed.ltp) * objpos.Qty * objcon.lotsize);
                        }
                        portfolio = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}", Utils.GetExch(objcon.exch), symbol, stringvalidity, buysell, objpos.Qty, price, pl, objfeed.ltp);//Exchange name_Symbol name_Validity_BuyorSellTrade_Qty_Price_ProfitLos_MktPirce
                                                                                                                                                                                      //This is one position, you will get multiple positions if available in this foreach loop, you can concat this string yourself to receive in the app
                        string[] res = portfolio.Split('_');
                        // result = result + "{\"message\":\"success\", \"exchange\":\"" + res[0] + "\",\"symbol\": \"" + res[1] + "\",\"validity\": \"" + res[2] + "\",\"buysell\": \"" + res[3] + "\",\"holdqty\": \"" + res[4] + "\",\"holdprice\": \"" + res[5] + "\",\"marketprice\": \"" + objfeed.ltp + "\",\"profitloss\": \"" + pl + "\"}";
                        li.Add(new portfolio { exchange = res[0], symbol = res[1], validity = res[2], buysell = res[3], holdqty = res[4], holdprice = res[5], marketprice = objfeed.ltp.ToString(), profitloss = pl.ToString() });
                        //Response.Write(result);
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

        [Route("api/Trade/cancelOrder")]
        [HttpPost]
        public HttpResponseMessage cancelOrder([FromBody] cancelorder co)
        {
            Utils _utils = new Utils();
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
                if (identity.Name != co.clientcode)
                {
                    cm.Message = "Unauthorized Access for Clientcode= " + co.clientcode;
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
                bool flag = _utils.ValidatePendingOrder(co.orderno, Conn);
                if (flag == false)
                {
                    cm.Message = "Order already executed, cannot cancel this order";
                    li.Add(new common { Message = cm.Message });

                }
                else if (!_utils.ValidateMarketTime(co.exch, ""))
                {
                    cm.Message = "Market is closed!!";
                    li.Add(new common { Message = cm.Message });
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand("CancelOrders", Conn) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Orderno", co.orderno);
                        cmd.Parameters.AddWithValue("@accountno", co.clientcode);

                        try
                        {
                            cmd.ExecuteNonQuery();
                            cm.Message = "Order Cancelled Successfully!!!";
                            li.Add(new common { Message = cm.Message });
                            //li.Add("Order Cancelled Successfully!!!");

                        }
                        catch (Exception ex)
                        {

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

        [Route("api/Trade/modifyOrder")]
        [HttpPost]
        public HttpResponseMessage modifyOrder([FromBody] modifyorder mo)
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
                if (feedConn.State != ConnectionState.Open)
                {
                    feedConn.Open();
                }
                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();
                }
                Trades objtrade = new Trades();
                if (_utils.GetOrderDetails(Conn, mo.orderno, ref objtrade))
                {
                    bool flag = _utils.ValidatePendingOrder(mo.orderno, Conn);
                    if (flag == false)
                    {
                        cm.Message = "Order already executed, cannot cancel this order";
                        li.Add(new common { Message = cm.Message });
                        response.data = li;
                        common.status = "Success";
                        common.result = li.Count().ToString() + " record found";
                        response.description = common;
                        JSonResponse js1 = new JSonResponse();
                        var object2 = js1.JSon(response);
                        return Request.CreateResponse(HttpStatusCode.OK, object2);
                    }
                    else if (!_utils.ValidateMarketTime(objtrade.exch, ""))
                    {
                        cm.Message = "Market is closed!!";
                        li.Add(new common { Message = cm.Message });
                        response.data = li;
                        common.status = "Success";
                        common.result = li.Count().ToString() + " record found";
                        response.description = common;
                        JSonResponse js1 = new JSonResponse();
                        var object2 = js1.JSon(response);
                        return Request.CreateResponse(HttpStatusCode.OK, object2);
                    }
                    else
                    {
                        Feeds objfeeds = _utils.GetDBFeed(objtrade.Symbol, feedConn);
                        Contracts objcon = _utils.GetContract(objtrade.Symbol, Conn);
                        User_info objuserinfo = _utils.GetUserinfo(objtrade.clientcode, Conn);
                        string exchh = Utils.GetExch(objcon.exch);
                        if (mo.price == 0)
                        {
                            if (objtrade.buysell == 1)
                                objtrade.price = objfeeds.ask;
                            else if (objtrade.buysell == 2)
                                objtrade.price = objfeeds.bid;
                            if (Utils.InsertPendingTrade(objtrade, Conn))
                            {

                                cm.Message = "Order modified Successfully!!";
                                li.Add(new common { Message = cm.Message });
                                response.data = li;
                                common.status = "Success";
                                common.result = li.Count().ToString() + " record found";
                                response.description = common;
                                JSonResponse js1 = new JSonResponse();
                                var object2 = js1.JSon(response);
                                return Request.CreateResponse(HttpStatusCode.OK, object2);
                            }
                            else
                            {
                                cm.Message = "Unable to modify Order.!!";
                                li.Add(new common { Message = cm.Message });
                                response.data = li;
                                common.status = "Success";
                                common.result = li.Count().ToString() + " record found";
                                response.description = common;


                                JSonResponse js1 = new JSonResponse();
                                var object2 = js1.JSon(response);
                                return Request.CreateResponse(HttpStatusCode.OK, object2);

                            }
                        }
                        else
                        {
                            if (mo.price > 0 && objuserinfo.offset == 1)
                            {
                                if (objuserinfo.OffsetExch.Contains(exchh))
                                {
                                    string responsemsg = string.Empty;
                                    if (!_utils.ValidateOffset(objfeeds, objtrade.buysell, mo.price, objtrade.producttype, objtrade.clientcode, objcon.symbol, objuserinfo, Conn, ref responsemsg))

                                    {

                                        cm.Message = responsemsg;
                                        li.Add(new common { Message = cm.Message });
                                        response.data = li;
                                        common.status = "Success";
                                        common.result = li.Count().ToString() + " record found";
                                        response.description = common;
                                        JSonResponse js1 = new JSonResponse();
                                        var object2 = js1.JSon(response);
                                        return Request.CreateResponse(HttpStatusCode.OK, object2);
                                    }

                                }
                            }
                            if (mo.price > 0 && objuserinfo.isHLTrading == 1)
                            {

                                if (objuserinfo.HighLowExch.Contains(exchh))
                                {
                                    if (objtrade.producttype == 1)
                                    {
                                        if (objtrade.buysell == 1 && mo.price >= objfeeds.low)
                                        {
                                            //return "Invalid price, order price not matching with offset defined(High-Low)";
                                            //li.Add("Invalid price, order price not matching with offset defined(High-Low)");
                                            cm.Message = "Invalid price, order price not matching with offset defined(High-Low)";
                                            li.Add(new common { Message = cm.Message });
                                            response.data = li;
                                            common.status = "Success";
                                            common.result = li.Count().ToString() + " record found";
                                            response.description = common;
                                            JSonResponse js1 = new JSonResponse();
                                            var object2 = js1.JSon(response);
                                            return Request.CreateResponse(HttpStatusCode.OK, object2);

                                        }
                                        else if (objtrade.buysell == 2 && mo.price <= objfeeds.high)
                                        {
                                            //return "Invalid price, order price not matching with offset defined(High-Low)";
                                            // li.Add("Invalid price, order price not matching with offset defined(High-Low)");
                                            cm.Message = "Invalid price, order price not matching with offset defined(High-Low)";
                                            li.Add(new common { Message = cm.Message });
                                            response.data = li;
                                            common.status = "Success";
                                            common.result = li.Count().ToString() + " record found";
                                            response.description = common;
                                            JSonResponse js1 = new JSonResponse();
                                            var object2 = js1.JSon(response);
                                            return Request.CreateResponse(HttpStatusCode.OK, object2);
                                        }
                                    }
                                    else
                                    {
                                        if (objtrade.buysell == 1 && mo.price <= objfeeds.high)
                                        {
                                            //return "Invalid price, order price not matching with offset defined(High-Low)";
                                            //li.Add("Invalid price, order price not matching with offset defined(High-Low)");
                                            cm.Message = "Invalid price, order price not matching with offset defined(High-Low)";
                                            li.Add(new common { Message = cm.Message });
                                            response.data = li;
                                            common.status = "Success";
                                            common.result = li.Count().ToString() + " record found";
                                            response.description = common;
                                            JSonResponse js1 = new JSonResponse();
                                            var object2 = js1.JSon(response);
                                            return Request.CreateResponse(HttpStatusCode.OK, object2);

                                        }
                                        if (objtrade.buysell == 2 && mo.price >= objfeeds.low)
                                        {
                                            //return "Invalid price, order price not matching with offset defined(High-Low)";
                                            //li.Add("Invalid price, order price not matching with offset defined(High-Low)");
                                            cm.Message = "Invalid price, order price not matching with offset defined(High-Low)";
                                            li.Add(new common { Message = cm.Message });
                                            response.data = li;
                                            common.status = "Success";
                                            common.result = li.Count().ToString() + " record found";
                                            response.description = common;
                                            JSonResponse js1 = new JSonResponse();
                                            var object2 = js1.JSon(response);
                                            return Request.CreateResponse(HttpStatusCode.OK, object2);
                                        }
                                    }

                                }
                            }

                            if (objtrade.producttype == 1) // RL 
                            {
                                if (objtrade.buysell == 1) // BUY
                                {
                                    if (mo.price >= objfeeds.ask)
                                        mo.price = objfeeds.ask;
                                }
                                else // SELL
                                {
                                    if (mo.price <= objfeeds.bid)
                                        mo.price = objfeeds.bid;
                                }
                            }

                            else if (objtrade.producttype == 2)// SL
                            {
                                if (objtrade.buysell == 1) // BUY
                                {
                                    if (objfeeds.ask >= mo.price)
                                    {
                                        //li.Add("Invalid SL price found.!!");
                                        cm.Message = "Invalid SL price found.!!";
                                        li.Add(new common { Message = cm.Message });
                                        response.data = li;
                                        common.status = "Success";
                                        common.result = li.Count().ToString() + " record found";
                                        response.description = common;
                                        JSonResponse js1 = new JSonResponse();
                                        var object2 = js1.JSon(response);
                                        return Request.CreateResponse(HttpStatusCode.OK, object2);
                                    }
                                }
                                else // SELL
                                {
                                    if (mo.price >= objfeeds.bid)
                                    {
                                        //li.Add("Invalid SL price found.!!");
                                        cm.Message = "Invalid SL price found.!!";
                                        li.Add(new common { Message = cm.Message });
                                        response.data = li;
                                        common.status = "Success";
                                        common.result = li.Count().ToString() + " record found";
                                        response.description = common;
                                        JSonResponse js1 = new JSonResponse();
                                        var object2 = js1.JSon(response);
                                        return Request.CreateResponse(HttpStatusCode.OK, object2);

                                    }
                                }
                            }
                        }
                        if (mo.price == 0)
                            mo.price = objtrade.price;
                        using (SqlCommand cmd = new SqlCommand("ModifyOrder", Conn) { CommandType = CommandType.StoredProcedure })
                        {
                            cmd.Parameters.AddWithValue("@clientcode", objtrade.clientcode);
                            cmd.Parameters.AddWithValue("@orderno", objtrade.orderno);
                            cmd.Parameters.AddWithValue("@oldqty", objtrade.qty);
                            cmd.Parameters.AddWithValue("@oldprice", objtrade.Ordprice);
                            cmd.Parameters.AddWithValue("@qty", mo.qty);
                            cmd.Parameters.AddWithValue("@price", mo.price);
                            cmd.Parameters.AddWithValue("@lastmodified", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                            try
                            {
                                int res = cmd.ExecuteNonQuery();
                                if (res > 0)
                                {
                                    //li.Add("Order modified Successfully!!");
                                    cm.Message = "Order modified Successfully!!";
                                    li.Add(new common { Message = cm.Message });
                                }
                            }
                            catch
                            {
                                // li.Add("Unable to modify Order.");
                                cm.Message = "Unable to modify Order.";
                                li.Add(new common { Message = cm.Message });
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


        [Route("api/Trade/getNetPos")]
        [HttpGet]
        public HttpResponseMessage getNetPos(string clientcode)
        {
            Utils _utils = new Utils();
            //netPosition np = new netPosition();

            SqlConnection feedConn = Utils.feedconn;
            SqlConnection Conn = Utils.conn;
            FRatesResponse1 response = new FRatesResponse1();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            List<dynamic> li1 = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
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
                response.description = common;
                var identity = (ClaimsIdentity)User.Identity;
                if (identity.Name != clientcode)
                {
                    cm.Message = "Unauthorized Access for Clientcode= " + clientcode;
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    JSonResponse js1 = new JSonResponse();
                    var object11 = js1.JSon1(response);
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
                Dictionary<string, buysellPos> _BuySellAvgPos = new Dictionary<string, buysellPos>();
                _utils.GetTradePosition(ref _BuySellAvgPos, clientcode, false);
                Dictionary<string, buysellnetpospfls> _NetPosPfLs = _utils.ProcessProfitLoss1(_BuySellAvgPos, Conn, clientcode);
                string portfolio = string.Empty;
                Decimal netmtm = 0, realisedpl = 0, unrealisedpl = 0;
                foreach (var items in _NetPosPfLs)
                {
                    string[] data = items.Key.Split('_');
                    string symbol = data[0];
                    string account = data[1];
                    int validity = Convert.ToInt32(data[2]);
                    Contracts objcon = _utils.GetContract(symbol, Conn);
                    buysellnetpospfls objpos = items.Value;
                    Feeds objfeed = _utils.GetDBFeed(symbol, feedConn);
                    string stringvalidity = "CNF";
                    if (validity == 2)
                        stringvalidity = "DAY";
                    //if (objpos.buy_sell > 0)
                    //{

                    //string buysell = "BUY";
                    //decimal price = Decimal.Round(objpos.buyprice, 2);
                    decimal mtm = 0;
                    int netqty = objpos.BQty - objpos.SQty;
                    if (objpos.BQty > objpos.SQty)
                    {

                        if (objcon.exch == 2)
                        {
                            if (objfeed.bid == 0)
                                mtm = Decimal.Round((objfeed.ltp - objpos.buyprice) * objpos.Qty);
                            else
                                mtm = Decimal.Round((objfeed.bid - objpos.buyprice) * objpos.Qty);
                        }
                        else
                        {
                            if (objfeed.bid == 0)
                                mtm = Decimal.Round((objfeed.ltp - objpos.buyprice) * objpos.Qty * objcon.lotsize);
                            else
                                mtm = Decimal.Round((objfeed.bid - objpos.buyprice) * objpos.Qty * objcon.lotsize);
                        }
                    }
                    else if (objpos.BQty < objpos.SQty)
                    {
                        //buysell = "SELL";
                        //price = Decimal.Round(objpos.sellprice, 2);

                        if (objcon.exch == 2)
                        {
                            if (objfeed.ask == 0)
                                mtm = Decimal.Round((objpos.sellprice - objfeed.ltp) * objpos.Qty);
                            else
                                mtm = Decimal.Round((objpos.sellprice - objfeed.ask) * objpos.Qty);
                        }
                        else
                        {
                            if (objfeed.ask == 0)
                                mtm = Decimal.Round((objpos.sellprice - objfeed.ltp) * objpos.Qty * objcon.lotsize);
                            else
                                mtm = Decimal.Round((objpos.sellprice - objfeed.ask) * objpos.Qty * objcon.lotsize);
                        }
                    }
                    realisedpl += objpos.p_l;
                    unrealisedpl += mtm;
                    netmtm = realisedpl + unrealisedpl;
                    //else if (objpos.buy_sell == 0)
                    //{
                    //    if (objcon.exch == 2)
                    //        pl = Decimal.Round((objpos.sellprice - objpos.buyprice) * objpos.BQty);
                    //    else
                    //        pl = Decimal.Round((objpos.sellprice - objpos.buyprice) * objpos.BQty * objcon.lotsize);
                    //}
                    //portfolio = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}", GetExch(objcon.exch), symbol, stringvalidity, buysell, objpos.Qty, price, pl, objfeed.ltp);//Exchange name_Symbol name_Validity_BuyorSellTrade_Qty_Price_ProfitLos_MktPirce
                    //This is one position, you will get multiple positions if available in this foreach loop, you can concat this string yourself to receive in the app
                    string[] res = portfolio.Split('_');
                    //result = result + "{\"message\":\"success\", \"exchange\":\"" + GetExch(objcon.exch) + "\",\"symbol\": \"" + symbol + "\",\"netqty\": \"" + netqty + "\",\"validity\": \"" + stringvalidity + "\",\"buyqty\": \"" + objpos.BQty + "\",\"buyrate\": \"" + objpos.buyprice + "\",\"sellqty\": \"" + objpos.SQty + "\",\"sellrate\": \"" + objpos.sellprice + "\",\"bookedpl\": \"" + objpos.p_l + "\",\"mtm\": \"" + mtm + "\"}";
                    li.Add(new netPosition { exch = Utils.GetExch(objcon.exch), symbol = symbol, netqty = netqty, stringvalidity = stringvalidity, BQty = objpos.BQty, buyprice = objpos.buyprice, SQty = objpos.SQty, sellprice = objpos.sellprice, p_l = objpos.p_l, mtm = mtm });
                }
                li1.Add(new topPanel { netmtm = netmtm, realisedpl = realisedpl, unrealisedpl = unrealisedpl });
                response.data = li;
                response.TopPanel = li1;
                common.status = "Success";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon1(response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }


        [Route("api/Trade/sqOffPosNew")]
        [HttpGet]
        public HttpResponseMessage sqOffPosNew(string clientcode)
        {
            int cnt = 0;
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
                Dictionary<string, buysellPos> _BuySellAvgPos = new Dictionary<string, buysellPos>();
                _utils.GetTradePosition(ref _BuySellAvgPos, clientcode, false);
                Dictionary<string, buysellnetpospfls> _NetPosPfLs = _utils.ProcessProfitLoss1(_BuySellAvgPos, Conn, clientcode);

                foreach (var items in _NetPosPfLs)
                {
                    string[] data = items.Key.Split('_');
                    string symbol = data[0];
                    string account = data[1];
                    int validity = Convert.ToInt32(data[2]);
                    Contracts objcon = _utils.GetContract(symbol, Conn);

                    buysellnetpospfls objpos = items.Value;
                    Feeds objfeed = _utils.GetDBFeed(symbol, feedConn);
                    if (!_utils.ValidateMarketTime(objcon.exch, "")) // Validate Market Timings
                    {
                        // result = result + "{\"message\":\"fail\", \"description\":\"Market is closed\"}";
                        // break;   
                        continue;
                    }

                    if (objpos.BQty > objpos.SQty)
                    {
                        int netqty = objpos.BQty - objpos.SQty;
                        if (_utils.SqOffPos(account, symbol, validity, 2, netqty, objcon, objfeed, Conn))
                            cnt += 1;
                    }
                    else if (objpos.BQty < objpos.SQty)
                    {
                        int netqty = objpos.SQty - objpos.BQty;
                        if (_utils.SqOffPos(account, symbol, validity, 1, netqty, objcon, objfeed, Conn))
                            cnt += 1;
                    }

                }

                cm.Message = cnt + " Positions closed";
                li.Add(new common { Message = cm.Message });

                response.data = li;
                common.status = "Success";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }

        [Route("api/Trade/validateTrade")]
        [HttpPost]
        public HttpResponseMessage validateTrade([FromBody] validatetrade vt)
        {
            Utils _utils = new Utils();
            JSonResponse js = new JSonResponse();
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
                //if (identity.Name != vt.clientcode)
                //{
                //    cm.Message = "Unauthorized Access for Clientcode= " + vt.clientcode;
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
                DateTime Servertime = DateTime.Now;
                User_info objuserinfo = _utils.GetUserinfo(vt.clientcode, Conn);
                Contracts objcon = _utils.GetContract(vt.symbol, Conn);
                string acc = vt.clientcode;
                int buysell = Int32.Parse(vt.operation);
                //int buysell = 1;
                //if (operation.ToUpper() == "SELL")
                //    buysell = 2;
                int ordertypee = Convert.ToInt32(vt.ordertype);
                string stringOrdType = "RL";
                if (ordertypee == 2)
                    stringOrdType = "SL";
                if (ordertypee == 3 || ordertypee == 0)
                    stringOrdType = "MKT";

                string exchh = Utils.GetExch(objcon.exch);
                if (!_utils.ValidateMarketTime(objcon.exch, objcon.symbol)) // Validate Market Timings
                {
                    //return "Market is closed!!";
                    //li.Add("Market is closed");
                    cm.Message = "Market is closed";
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    common.status = "Failed";
                    common.result = li.Count().ToString() + " record found";
                    response.description = common;
                    //JSonResponse js = new JSonResponse();
                    var object00 = js.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.OK, object00);

                }

                if ((Convert.ToInt32(vt.qty) % objcon.lotsize != 0) && (exchh == "NSEFUT" || exchh == "NSEOPT") && objuserinfo.oddlot == 0)
                {
                    //li.Add("Qty does not matches with Lotsize");
                    cm.Message = "Qty does not matches with Lotsize";
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    common.status = "Failed";
                    common.result = li.Count().ToString() + " record found";
                    response.description = common;
                    //JSonResponse js = new JSonResponse();
                    var object01 = js.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.OK, object01);

                }

                if (!_utils.ValidateContractStatus(objcon.SymDesp, 3, objuserinfo, objcon.exch, Conn)) // Validate Contract Suspended Status
                {
                    //return "Contract Suspended by Admin";
                    //li.Add("Contract Suspended by Admin");
                    cm.Message = "Contract Suspended by Admin";
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    common.status = "Failed";
                    common.result = li.Count().ToString() + " record found";
                    response.description = common;
                    // JSonResponse js = new JSonResponse();
                    var object02 = js.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.OK, object02);

                }

                if (objuserinfo.marginstatus == 0 && objuserinfo.usertype == 4) // Validate Margin Status -- Blocked or Unblocked
                {
                    //return "Margin Blocked by Admin";
                    //li.Add("Margin Blocked by Admin");
                    cm.Message = "Margin Blocked by Admin";
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    common.status = "Failed";
                    common.result = li.Count().ToString() + " record found";
                    response.description = common;
                    // JSonResponse js = new JSonResponse();
                    var object03 = js.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.OK, object03);

                }
                // Collect Order Parameters
                Trades objord = new Trades()
                {
                    exch = objcon.exch,
                    Symbol = vt.symbol,
                    buysell = buysell,
                    producttype = ordertypee,
                    qty = Convert.ToInt32(vt.qty),
                    orderno = Utils.OrderNoGenerate(),
                    validity = (int)Enum.Parse(typeof(Utils.ValidityType), vt.validity.ToUpper()),
                    //userremarks = txtUserRemarks.Text,
                    clientcode = acc,
                    traderid = vt.clientcode,
                    Createon = Servertime,
                    Lastmodified = Servertime,
                    macIP = vt.macip// GetIPAddress()
                };
                Limits objlimits = _utils.GetLimits(objord.clientcode, Conn);

                if (!objlimits.possitionValidity.Contains(_utils.GetStringValidity(objord.validity)))
                {
                    //li.Add("Validity not allowed for Client.");
                    cm.Message = "Validity not allowed for Client.";
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    common.status = "Failed";
                    common.result = li.Count().ToString() + " record found";
                    response.description = common;
                    // JSonResponse js = new JSonResponse();
                    var object03 = js.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.OK, object03);

                }

                if (!objlimits.Productype.Contains(stringOrdType.ToUpper()))
                {
                    // li.Add("Order Type not allowed for Client.");
                    cm.Message = "Order Type not allowed for Client.";
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    common.status = "Failed";
                    common.result = li.Count().ToString() + " record found";
                    response.description = common;
                    // JSonResponse js = new JSonResponse();
                    var object03 = js.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.OK, object03);

                }

                if (objlimits.cashmrgn <= 0) // Validate Cash margin
                {
                    //return "Cash Margin Not Available for Trading";
                    //li.Add("Cash Margin Not Available for Trading");
                    cm.Message = "Cash Margin Not Available for Trading";
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    common.status = "Failed";
                    common.result = li.Count().ToString() + " record found";
                    response.description = common;
                    // JSonResponse js = new JSonResponse();
                    var object03 = js.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.OK, object03);

                }

                SymbolMargin objmrgn = _utils.GetSymbolwiseMrgn(objuserinfo, objcon.symbol, Conn);
                if (objlimits.brkupType == 2)
                {
                    if (objcon.exch == 2)
                    {
                        if (objuserinfo.oddlot == 0)
                        {
                            int finalqty = objmrgn.brkupQty * objcon.lotsize;
                            if (objord.qty > finalqty) // Validate Breakup
                            {
                                //li.Add("Qty Exceeding Breakup Limit, Breakup Defined is: " + objmrgn.brkupQty + " Qty/Lots");
                                cm.Message = "Qty Exceeding Breakup Limit, Breakup Defined is: " + objmrgn.brkupQty + " Qty/Lots";
                                li.Add(new common { Message = cm.Message });
                                response.data = li;
                                common.status = "Failed";
                                common.result = li.Count().ToString() + " record found";
                                response.description = common;
                                // JSonResponse js = new JSonResponse();
                                var object03 = js.JSon(response);
                                return Request.CreateResponse(HttpStatusCode.OK, object03);

                            }
                        }
                        else
                        {
                            if (objord.qty > objmrgn.brkupQty) // Validate Breakup
                            {
                                //li.Add("Qty Exceeding Breakup Limit, Breakup Defined is: " + objmrgn.brkupQty + " Qty/Lots");
                                cm.Message = "Qty Exceeding Breakup Limit, Breakup Defined is: " + objmrgn.brkupQty + " Qty/Lots";
                                li.Add(new common { Message = cm.Message });
                                response.data = li;
                                common.status = "Failed";
                                common.result = li.Count().ToString() + " record found";
                                response.description = common;
                                // JSonResponse js = new JSonResponse();
                                var object03 = js.JSon(response);
                                return Request.CreateResponse(HttpStatusCode.OK, object03);

                            }
                        }

                    }
                    else
                    {
                        if (objord.qty > objmrgn.brkupQty) // Validate Breakup
                        {
                            // li.Add("Qty Exceeding Breakup Limit, Breakup Defined is: " + objmrgn.brkupQty + " Qty/Lots");
                            cm.Message = "Qty Exceeding Breakup Limit, Breakup Defined is: " + objmrgn.brkupQty + " Qty/Lots";
                            li.Add(new common { Message = cm.Message });
                            response.data = li;
                            common.status = "Failed";
                            common.result = li.Count().ToString() + " record found";
                            response.description = common;
                            // JSonResponse js = new JSonResponse();
                            var object03 = js.JSon(response);
                            return Request.CreateResponse(HttpStatusCode.OK, object03);
                        }
                    }

                }
                else
                {
                    int breakupQty = _utils.getBreakup(objcon, objlimits);
                    if (objord.qty > breakupQty) // Validate Breakup
                    {
                        switch (objord.exch)
                        {
                            case 1:
                                {
                                    //li.Add("Qty Exceeding Breakup Limit, Breakup Defined is: " + objlimits.McxBrkup + " Lots");
                                    cm.Message = "Qty Exceeding Breakup Limit, Breakup Defined is: " + objlimits.McxBrkup + " Lots";
                                    li.Add(new common { Message = cm.Message });
                                    response.data = li;
                                    common.status = "Failed";
                                    common.result = li.Count().ToString() + " record found";
                                    response.description = common;
                                    // JSonResponse js = new JSonResponse();
                                    var object03 = js.JSon(response);
                                    return Request.CreateResponse(HttpStatusCode.OK, object03);
                                }



                            case 2:
                                {
                                    //li.Add("Qty Exceeding Breakup Limit, Breakup Defined is: " + objlimits.NsefutBrkup + " Lots");
                                    cm.Message = "Qty Exceeding Breakup Limit, Breakup Defined is: " + objlimits.NsefutBrkup + " Lots";
                                    li.Add(new common { Message = cm.Message });
                                    response.data = li;
                                    common.status = "Failed";
                                    common.result = li.Count().ToString() + " record found";
                                    response.description = common;
                                    // JSonResponse js = new JSonResponse();
                                    var object03 = js.JSon(response);
                                    return Request.CreateResponse(HttpStatusCode.OK, object03);
                                }

                            case 3:
                                {
                                    // li.Add("Qty Exceeding Breakup Limit, Breakup Defined is: " + objlimits.NcdexBrkup + " Lots");
                                    cm.Message = "Qty Exceeding Breakup Limit, Breakup Defined is: " + objlimits.NcdexBrkup + " Lots";
                                    li.Add(new common { Message = cm.Message });
                                    response.data = li;
                                    common.status = "Failed";
                                    common.result = li.Count().ToString() + " record found";
                                    response.description = common;
                                    // JSonResponse js = new JSonResponse();
                                    var object03 = js.JSon(response);
                                    return Request.CreateResponse(HttpStatusCode.OK, object03);
                                }


                            case 4:
                                {
                                    // li.Add("Qty Exceeding Breakup Limit, Breakup Defined is: " + objlimits.NsecurBrkup + " Lots");
                                    cm.Message = "Qty Exceeding Breakup Limit, Breakup Defined is: " + objlimits.NsecurBrkup + " Lots";
                                    li.Add(new common { Message = cm.Message });
                                    response.data = li;
                                    common.status = "Failed";
                                    common.result = li.Count().ToString() + " record found";
                                    response.description = common;
                                    // JSonResponse js = new JSonResponse();
                                    var object03 = js.JSon(response);
                                    return Request.CreateResponse(HttpStatusCode.OK, object03);
                                }

                        }
                        //return;
                        //return "Qty Exceeding Breakup Limit, Breakup Defined is: " + breakupQty + " Lots";

                    }
                }
                if (objlimits.tradeattributes == 3) // Validate Tradeattributes
                {
                    //return "Trading Blocked by Admin, Contact Admin";
                    //li.Add("Trading Blocked by Admin, Contact Admin");
                    cm.Message = "Trading Blocked by Admin, Contact Admin";
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    common.status = "Failed";
                    common.result = li.Count().ToString() + " record found";
                    response.description = common;
                    // JSonResponse js = new JSonResponse();
                    var object03 = js.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.OK, object03);
                }

                decimal tmpPrice = Decimal.Round(Convert.ToDecimal(vt.price), Utils.GetRoundoff(objcon.tickprice));
                if (vt.price.ToString().Contains("."))
                {
                    string[] lastdigits = vt.price.Split('.');
                    if (lastdigits[1].Length == 2)
                    {
                        if (Utils.GetRoundoff(objcon.tickprice) == 2)
                        {

                            int lastdigit = Convert.ToInt32(tmpPrice.ToString().Substring(tmpPrice.ToString().Length - 1, 1));

                            if (lastdigit > 5)
                            {
                                tmpPrice = decimal.Round(Convert.ToDecimal(tmpPrice.ToString().Substring(0, tmpPrice.ToString().Length - 1)), 2);
                                tmpPrice += 0.1M;
                            }
                            else if (lastdigit < 5 && lastdigit > 0)
                                tmpPrice = Convert.ToDecimal(tmpPrice.ToString().Substring(0, tmpPrice.ToString().Length - 1) + "5");
                        }
                    }
                }

                Feeds objfeeds = _utils.GetDBFeed(objord.Symbol, feedConn);// Validation with Market Feeds
                if (objfeeds.volume <= 0)
                {
                    // li.Add("Volume not available for trading");
                    cm.Message = "Volume not available for trading";
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    common.status = "Failed";
                    common.result = li.Count().ToString() + " record found";
                    response.description = common;
                    // JSonResponse js = new JSonResponse();
                    var object03 = js.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.OK, object03);

                }
                double DPRLimit = _utils.GetDPRLimit(objord.Symbol, objuserinfo.createdby, Conn);
                double DPRdiff = Math.Round((Convert.ToDouble(objfeeds.close) * DPRLimit) / 100, 2);
                double Upperlimit = Convert.ToDouble(objfeeds.close) + DPRdiff;
                double Lowerlimit = Convert.ToDouble(objfeeds.close) - DPRdiff;
                if (tmpPrice > 0 && DPRLimit > 0)
                {
                    if (objord.producttype == 1)
                    {
                        if (objord.buysell == 1 && (Convert.ToDouble(tmpPrice) < Lowerlimit))
                        {
                            //return "Price exceeding DPR Limit";
                            //li.Add("Price exceeding DPR Limit");
                            cm.Message = "Price exceeding DPR Limit";
                            li.Add(new common { Message = cm.Message });
                            response.data = li;
                            common.status = "Failed";
                            common.result = li.Count().ToString() + " record found";
                            response.description = common;
                            // JSonResponse js = new JSonResponse();
                            var object03 = js.JSon(response);
                            return Request.CreateResponse(HttpStatusCode.OK, object03);

                        }
                        else if (objord.buysell == 2 && (Convert.ToDouble(tmpPrice) > Upperlimit))
                        {
                            //return "Price exceeding DPR Limit";
                            //li.Add("Price exceeding DPR Limit");
                            cm.Message = "Price exceeding DPR Limit";
                            li.Add(new common { Message = cm.Message });
                            response.data = li;
                            common.status = "Failed";
                            common.result = li.Count().ToString() + " record found";
                            response.description = common;
                            // JSonResponse js = new JSonResponse();
                            var object03 = js.JSon(response);
                            return Request.CreateResponse(HttpStatusCode.OK, object03);

                        }
                    }
                    else if (objord.producttype == 2)
                    {
                        if (objord.buysell == 1 && (Convert.ToDouble(tmpPrice) < Upperlimit))
                        {
                            //return "Price exceeding DPR Limit";
                            //li.Add("Price exceeding DPR Limit");
                            cm.Message = "Price exceeding DPR Limit";
                            li.Add(new common { Message = cm.Message });
                            response.data = li;
                            common.status = "Failed";
                            common.result = li.Count().ToString() + " record found";
                            response.description = common;
                            // JSonResponse js = new JSonResponse();
                            var object03 = js.JSon(response);
                            return Request.CreateResponse(HttpStatusCode.OK, object03);

                        }
                        else if (objord.buysell == 2 && (Convert.ToDouble(tmpPrice) > Lowerlimit))
                        {
                            //return "Price exceeding DPR Limit";
                            //li.Add("Price exceeding DPR Limit");
                            cm.Message = "Price exceeding DPR Limit";
                            li.Add(new common { Message = cm.Message });
                            response.data = li;
                            common.status = "Failed";
                            common.result = li.Count().ToString() + " record found";
                            response.description = common;
                            // JSonResponse js = new JSonResponse();
                            var object03 = js.JSon(response);
                            return Request.CreateResponse(HttpStatusCode.OK, object03);

                        }
                    }

                }
                if (tmpPrice > 0 && objuserinfo.offset == 1)
                {
                    if (objuserinfo.OffsetExch.Contains(exchh))
                    {
                        string responsemsg = string.Empty;
                        if (!_utils.ValidateOffset(objfeeds, objord.buysell, tmpPrice, objord.producttype, objord.clientcode, objcon.symbol, objuserinfo, Conn, ref responsemsg))
                        {
                            //li.Add(responsemsg);
                            cm.Message = responsemsg;
                            li.Add(new common { Message = cm.Message });
                            response.data = li;
                            common.status = "Failed";
                            common.result = li.Count().ToString() + " record found";
                            response.description = common;
                            // JSonResponse js = new JSonResponse();
                            var object03 = js.JSon(response);
                            return Request.CreateResponse(HttpStatusCode.OK, object03);
                        }
                        //return responsemsg;
                    }
                    else if (tmpPrice > 0 && objuserinfo.isHLTrading == 1)
                    {
                        if (objuserinfo.HighLowExch.Contains(exchh))
                        {
                            if (objord.producttype == 1)
                            {
                                if (objord.buysell == 1 && tmpPrice >= objfeeds.low)
                                {
                                    //return "Invalid price, order price not matching with offset defined(High-Low)";
                                    //li.Add("Invalid price, order price not matching with offset defined(High-Low)");
                                    cm.Message = "Invalid price, order price not matching with offset defined(High-Low)";
                                    li.Add(new common { Message = cm.Message });
                                    response.data = li;
                                    common.status = "Failed";
                                    common.result = li.Count().ToString() + " record found";
                                    response.description = common;
                                    // JSonResponse js = new JSonResponse();
                                    var object03 = js.JSon(response);
                                    return Request.CreateResponse(HttpStatusCode.OK, object03);

                                }
                                else if (objord.buysell == 2 && tmpPrice <= objfeeds.high)
                                {
                                    //return "Invalid price, order price not matching with offset defined(High-Low)";
                                    //li.Add("Invalid price, order price not matching with offset defined(High-Low)");
                                    cm.Message = "Invalid price, order price not matching with offset defined(High-Low)";
                                    li.Add(new common { Message = cm.Message });
                                    response.data = li;
                                    common.status = "Failed";
                                    common.result = li.Count().ToString() + " record found";
                                    response.description = common;
                                    // JSonResponse js = new JSonResponse();
                                    var object03 = js.JSon(response);
                                    return Request.CreateResponse(HttpStatusCode.OK, object03);

                                }
                            }
                            else
                            {
                                if (objord.buysell == 1 && tmpPrice <= objfeeds.high)
                                {
                                    //return "Invalid price, order price not matching with offset defined(High-Low)";
                                    // li.Add("Invalid price, order price not matching with offset defined(High-Low)");
                                    cm.Message = "Invalid price, order price not matching with offset defined(High-Low)";
                                    li.Add(new common { Message = cm.Message });
                                    response.data = li;
                                    common.status = "Failed";
                                    common.result = li.Count().ToString() + " record found";
                                    response.description = common;
                                    // JSonResponse js = new JSonResponse();
                                    var object03 = js.JSon(response);
                                    return Request.CreateResponse(HttpStatusCode.OK, object03);

                                }
                                if (objord.buysell == 2 && tmpPrice >= objfeeds.low)
                                {
                                    //return "Invalid price, order price not matching with offset defined(High-Low)";
                                    //li.Add("Invalid price, order price not matching with offset defined(High-Low)");
                                    cm.Message = "Invalid price, order price not matching with offset defined(High-Low)";
                                    li.Add(new common { Message = cm.Message });
                                    response.data = li;
                                    common.status = "Failed";
                                    common.result = li.Count().ToString() + " record found";
                                    response.description = common;
                                    // JSonResponse js = new JSonResponse();
                                    var object03 = js.JSon(response);
                                    return Request.CreateResponse(HttpStatusCode.OK, object03);

                                }
                            }

                        }
                    }
                }
                else if (tmpPrice > 0 && objuserinfo.isHLTrading == 1)
                {
                    if (objuserinfo.HighLowExch.Contains(exchh))
                    {
                        if (objord.producttype == 1)
                        {
                            if (objord.buysell == 1 && tmpPrice >= objfeeds.low)
                            {
                                //return "Invalid price, order price not matching with offset defined(High-Low)";
                                //li.Add("Invalid price, order price not matching with offset defined(High-Low)");
                                cm.Message = "Invalid price, order price not matching with offset defined(High-Low)";
                                li.Add(new common { Message = cm.Message });
                                response.data = li;
                                common.status = "Failed";
                                common.result = li.Count().ToString() + " record found";
                                response.description = common;
                                // JSonResponse js = new JSonResponse();
                                var object03 = js.JSon(response);
                                return Request.CreateResponse(HttpStatusCode.OK, object03);

                            }
                            else if (objord.buysell == 2 && tmpPrice <= objfeeds.high)
                            {
                                //return "Invalid price, order price not matching with offset defined(High-Low)";
                                //li.Add("Invalid price, order price not matching with offset defined(High-Low)");
                                cm.Message = "Invalid price, order price not matching with offset defined(High-Low)";
                                li.Add(new common { Message = cm.Message });
                                response.data = li;
                                common.status = "Failed";
                                common.result = li.Count().ToString() + " record found";
                                response.description = common;
                                // JSonResponse js = new JSonResponse();
                                var object03 = js.JSon(response);
                                return Request.CreateResponse(HttpStatusCode.OK, object03);

                            }
                        }
                        else
                        {
                            if (objord.buysell == 1 && tmpPrice <= objfeeds.high)
                            {
                                //return "Invalid price, order price not matching with offset defined(High-Low)";
                                // li.Add("Invalid price, order price not matching with offset defined(High-Low)");
                                cm.Message = "Invalid price, order price not matching with offset defined(High-Low)";
                                li.Add(new common { Message = cm.Message });
                                response.data = li;
                                common.status = "Failed";
                                common.result = li.Count().ToString() + " record found";
                                response.description = common;
                                // JSonResponse js = new JSonResponse();
                                var object03 = js.JSon(response);
                                return Request.CreateResponse(HttpStatusCode.OK, object03);

                            }
                            if (objord.buysell == 2 && tmpPrice >= objfeeds.low)
                            {
                                //return "Invalid price, order price not matching with offset defined(High-Low)";
                                //li.Add("Invalid price, order price not matching with offset defined(High-Low)");
                                cm.Message = "Invalid price, order price not matching with offset defined(High-Low)";
                                li.Add(new common { Message = cm.Message });
                                response.data = li;
                                common.status = "Failed";
                                common.result = li.Count().ToString() + " record found";
                                response.description = common;
                                // JSonResponse js = new JSonResponse();
                                var object03 = js.JSon(response);
                                return Request.CreateResponse(HttpStatusCode.OK, object03);

                            }
                        }

                    }
                }
                if (!vt.IsModify)
                {
                    if (tmpPrice == 0 || objlimits.Productype.ToUpper() == "MKT") // MP
                    {
                        if (objord.buysell == 1)
                        {
                            objord.Ordprice = objfeeds.ask;
                            objord.price = objfeeds.ask;
                        }
                        else
                        {
                            objord.Ordprice = objfeeds.bid;
                            objord.price = objfeeds.bid;
                        }

                        objord.ordstatus = 1;
                        objord.exectype = 2;
                    }
                    else
                    {
                        objord.Ordprice = tmpPrice;
                        if (objord.producttype == 1) // RL 
                        {
                            if (objord.buysell == 1) // BUY
                            {
                                if (objord.Ordprice >= objfeeds.ask)
                                {
                                    objord.price = objfeeds.ask;

                                    objord.ordstatus = 1;
                                    objord.exectype = 2;
                                }
                                else if (objord.Ordprice < objfeeds.ask)
                                {
                                    objord.ordstatus = 2;
                                    objord.exectype = 1;
                                }

                            }
                            else // SELL
                            {
                                if (objord.Ordprice <= objfeeds.bid)
                                {
                                    objord.price = objfeeds.bid;

                                    objord.ordstatus = 1;
                                    objord.exectype = 2;
                                }
                                else if (objord.Ordprice > objfeeds.bid)
                                {
                                    objord.ordstatus = 2;
                                    objord.exectype = 1;
                                }
                            }
                        }
                        else if (objord.producttype == 2)// SL
                        {
                            if (objord.buysell == 1) // BUY
                            {
                                if (objfeeds.ask >= objord.Ordprice)
                                {
                                    //return "Invalid SL price found.";
                                    //li.Add("Invalid SL price found");
                                    cm.Message = "Invalid SL price found";
                                    li.Add(new common { Message = cm.Message });
                                    response.data = li;
                                    common.status = "Failed";
                                    common.result = li.Count().ToString() + " record found";
                                    response.description = common;
                                    // JSonResponse js = new JSonResponse();
                                    var object03 = js.JSon(response);
                                    return Request.CreateResponse(HttpStatusCode.OK, object03);
                                }
                                else
                                {
                                    objord.ordstatus = 2;
                                    objord.exectype = 1;
                                }
                            }
                            else // SELL
                            {
                                if (objord.Ordprice >= objfeeds.bid)
                                {
                                    //return "Invalid SL price found.";
                                    // li.Add("Invalid SL price found");
                                    cm.Message = "Invalid SL price found";
                                    li.Add(new common { Message = cm.Message });
                                    response.data = li;
                                    common.status = "Failed";
                                    common.result = li.Count().ToString() + " record found";
                                    response.description = common;
                                    // JSonResponse js = new JSonResponse();
                                    var object03 = js.JSon(response);
                                    return Request.CreateResponse(HttpStatusCode.OK, object03);
                                }
                                else
                                {
                                    objord.ordstatus = 2;
                                    objord.exectype = 1;
                                }
                            }
                        }
                    }
                }
                else
                {
                    objord.Ordprice = tmpPrice;
                }
                int sumPendIntraLots = 0;
                int sumPendCnfLots = 0;
                double currentmargin = 0;
                int curintralots = 0, curcnflots = 0, Netlots = 0;
                double pendingmargin = 0;
                double profitloss = 0;
                double RealisedPL = 0;
                double MrgnUtilised = 0;
                int SumIntraLots = 0;
                int SumCnfLots = 0;
                int TotintraLots = 0;
                int symlots = 0;
                bool isClosePos = false;
                double pendingturnover = 0;
                double turnoverUtilised = 0;
                double currentTurnover = 0;
                int exchlots = 0, ExchPendingLots = 0;

                if (objcon.exch == 2 || objcon.exch == 5)
                {
                    currentTurnover += objord.qty * Convert.ToDouble(objord.Ordprice);
                    int lots = (objord.qty / objcon.lotsize);
                    if (objuserinfo.oddlot == 1)
                        lots = objord.qty;
                    if (objord.validity == 1)
                    {

                        curcnflots = lots;
                        //currentmargin = Convert.ToDouble(objord.Ordprice) * lots;
                        currentmargin = objmrgn.delvmrgn * lots;
                    }
                    else
                    {
                        curintralots = lots;
                        //currentmargin = Convert.ToDouble(objord.Ordprice) * lots;
                        currentmargin = objmrgn.intramrgn * lots;
                    }
                }
                else
                {
                    currentTurnover += objord.qty * objcon.lotsize * Convert.ToDouble(objord.Ordprice);
                    if (objord.validity == 1)
                    {
                        curcnflots = objord.qty;
                        currentmargin = objmrgn.delvmrgn * objord.qty;
                    }
                    else
                    {
                        curintralots = objord.qty;
                        currentmargin = objmrgn.intramrgn * objord.qty;
                    }
                }
                int pendinglots = 0;
                _utils.GetPendingMargin(ref sumPendIntraLots, ref sumPendCnfLots, ref pendingmargin, ref pendingturnover, objord.Symbol, objord.clientcode, objord.exch, ref ExchPendingLots, objcon, objmrgn, objlimits, objuserinfo, objfeeds, ref pendinglots, objord.buysell);
                _utils.GetMarginUtlised(objord, objuserinfo, ref SumIntraLots, ref SumCnfLots, ref MrgnUtilised, ref profitloss, ref TotintraLots, ref isClosePos, ref RealisedPL,
                    ref turnoverUtilised, ref symlots, ref exchlots, objfeeds, objmrgn, objlimits, pendinglots);
                int netSymLots;
                int netintralots;
                int NetExchLots = 0;
                int _currentlots = objord.qty;//= (exchlots + ExchPendingLots + currentlots);
                if ((objcon.exch == 2 || objcon.exch == 5) && objuserinfo.oddlot == 0)
                    _currentlots = objord.qty / objcon.lotsize;
                if (objord.validity != 1)
                {
                    netintralots = sumPendIntraLots + SumCnfLots + curintralots + sumPendCnfLots;
                    if (isClosePos == false)
                    {
                        netSymLots = symlots + curcnflots + netintralots;
                        NetExchLots = (exchlots + ExchPendingLots + _currentlots);
                    }

                    else
                    {
                        netSymLots = symlots - curcnflots - netintralots;
                        int qy = netSymLots * 2;
                        qy = qy * -1;
                        NetExchLots = qy;
                        // NetExchLots = NetExchLots * -1;
                        netSymLots = netSymLots * -1;
                    }
                }
                else
                {
                    netintralots = sumPendCnfLots + SumIntraLots + curintralots + sumPendIntraLots;
                    if (isClosePos == false)
                    {
                        netSymLots = symlots + curcnflots + netintralots;
                        //exchlots = exchlots + objord.qty;
                        NetExchLots = (exchlots + ExchPendingLots + _currentlots);
                    }

                    else
                    {

                        netSymLots = symlots - curcnflots - netintralots;

                        // exchlots = exchlots - objord.qty;
                        int qy = netSymLots * 2;
                        qy = qy * -1;
                        NetExchLots = qy;
                        // NetExchLots = NetExchLots * -1;
                        netSymLots = netSymLots * -1;

                    }
                }

                //int netintralots = sumPendIntraLots + SumIntraLots + curintralots;
                //int netSymLots = symlots + curcnflots + netintralots;
                TotintraLots += curintralots;

                double TotTurnover = 0;
                if (objord.buysell == 1)
                {
                    //TotTurnover = pendingturnover + turnoverUtilised + currentTurnover;
                    if (!isClosePos)
                        TotTurnover = pendingturnover + turnoverUtilised + currentTurnover;
                    else//
                    {
                        TotTurnover = -pendingturnover + turnoverUtilised - currentTurnover;
                        if (TotTurnover < 0)
                            TotTurnover = TotTurnover * -1;
                    }//
                }
                else
                {
                    //TotTurnover = pendingturnover + turnoverUtilised - currentTurnover;
                    //TotTurnover = TotTurnover * -1;
                    if (!isClosePos)//
                        TotTurnover = pendingturnover + turnoverUtilised + currentTurnover;//
                    else
                    {
                        TotTurnover = -pendingturnover + turnoverUtilised - currentTurnover;
                        if (TotTurnover < 0)
                            TotTurnover = TotTurnover * -1;
                    }
                }

                int netcnflots = sumPendCnfLots + SumCnfLots + curcnflots;
                Netlots = netintralots + netcnflots;
                //double netmargin = Math.Round(MrgnUtilised + pendingmargin + currentmargin - RealisedPL);
                double netmargin = Math.Round(MrgnUtilised + pendingmargin + currentmargin);
                int currentlots = objord.qty;
                if ((objcon.exch == 2 || objcon.exch == 5) && objuserinfo.oddlot == 0)
                    currentlots = objord.qty / objcon.lotsize;
                else
                    curcnflots = objord.qty;


                if (!isClosePos)
                {
                    string respnse = string.Empty;
                    if (!_utils.ValidateMargin(objord, TotTurnover, objlimits, objuserinfo, NetExchLots, netmargin, netSymLots, ref respnse, objmrgn))
                    {
                        //li.Add(respnse);
                        cm.Message = respnse;
                        li.Add(new common { Message = cm.Message });
                        response.data = li;
                        common.status = "Failed";
                        common.result = li.Count().ToString() + " record found";
                        response.description = common;
                        // JSonResponse js = new JSonResponse();
                        var object03 = js.JSon(response);
                        return Request.CreateResponse(HttpStatusCode.OK, object03);
                    }
                    //return respnse;
                    if (!_utils.ValidateContractStatus(objcon.SymDesp, 2, objuserinfo, objcon.exch, Conn)) // Validate Contract Banned Status
                    {
                        //return "Contract banned by Admin";
                        //li.Add("Contract banned by Admin");
                        cm.Message = "Contract banned by Admin";
                        li.Add(new common { Message = cm.Message });
                        response.data = li;
                        common.status = "Failed";
                        common.result = li.Count().ToString() + " record found";
                        response.description = common;
                        // JSonResponse js = new JSonResponse();
                        var object03 = js.JSon(response);
                        return Request.CreateResponse(HttpStatusCode.OK, object03);
                    }

                    if (!_utils.ValidateContractStatus(objcon.SymDesp, 3, objuserinfo, objcon.exch, Conn)) // Validate Contract Suspended Status
                    {
                        //return "Contract Suspended by Admin";
                        //li.Add("Contract Suspended by Admin");
                        cm.Message = "Contract Suspended by Admin";
                        li.Add(new common { Message = cm.Message });
                        response.data = li;
                        common.status = "Failed";
                        common.result = li.Count().ToString() + " record found";
                        response.description = common;
                        // JSonResponse js = new JSonResponse();
                        var object03 = js.JSon(response);
                        return Request.CreateResponse(HttpStatusCode.OK, object03);
                    }

                    if (!_utils.ValidateContractStatus(objcon.SymDesp, 4, objuserinfo, objcon.exch, Conn)) // Validate Contract Status for Exch Ban
                    {
                        //return "Contract in Exchange Banned Period, you can only close Open Positions in this Contract";
                        // li.Add("Contract in Exchange Banned Period, you can only close Open Positions in this Contract");
                        cm.Message = "Contract in Exchange Banned Period, you can only close Open Positions in this Contract";
                        li.Add(new common { Message = cm.Message });
                        response.data = li;
                        common.status = "Failed";
                        common.result = li.Count().ToString() + " record found";
                        response.description = common;
                        // JSonResponse js = new JSonResponse();
                        var object03 = js.JSon(response);
                        return Request.CreateResponse(HttpStatusCode.OK, object03);
                    }
                }
                else
                {
                    //if (objord.buysell == 1)
                    //    NetExchLots = (exchlots + ExchPendingLots + currentlots);
                    //else
                    //    NetExchLots = (exchlots + ExchPendingLots - currentlots);
                    double netmargin1 = Math.Round(MrgnUtilised + pendingmargin - currentmargin);
                    if (netmargin1 < 0)
                        netmargin1 = netmargin1 * -1;
                    string respnse = string.Empty;
                    if (!_utils.ValidateMarginsqoff(objord, TotTurnover, objlimits, objuserinfo, NetExchLots, netmargin1, netSymLots, ref respnse, objmrgn))
                    {
                        // li.Add( respnse);
                        cm.Message = respnse;
                        li.Add(new common { Message = cm.Message });
                        response.data = li;
                        common.status = "Failed";
                        common.result = li.Count().ToString() + " record found";
                        response.description = common;
                        // JSonResponse js = new JSonResponse();
                        var object03 = js.JSon(response);
                        return Request.CreateResponse(HttpStatusCode.OK, object03);
                    }
                    //if (!_utils.ValidatPositionCloseTime(objord, objcon, Servertime))
                    //{
                    //    // objmain.DisplayMessage("Not allowed to close position within time period defined by admin", 2);
                    //    //  objmain.insertSurveillanceMessages(acc, "Not allowed to close position within time period defined by admin, Symbol " + objord.Symbol + " clientcode " + objord.clientcode + "", Serverdt);
                    //    // return;
                    //   li.Add("Not allowed to close position within time period defined by admin");
                    //   response.data = li;
                    //    common.status = "Failed";
                    //    common.result = li.Count().ToString() + " record found";
                    //    response.description = common;
                    //    // JSonResponse js = new JSonResponse();
                    //    var object03 = js.JSon(response);
                    //    return Request.CreateResponse(HttpStatusCode.OK, object03);
                    //}
                }
                if (vt.IsModify)
                {
                    //decimal modifyprice = Decimal.Round(Convert.ToDecimal(), GetRoundoff(objcon.tickprice));
                    decimal modifyprice = Decimal.Round(Convert.ToDecimal(vt.price), Utils.GetRoundoff(objcon.tickprice));
                    if (modifyprice.ToString().Contains("."))
                    {
                        string[] lastdigits = vt.price.Split('.');
                        if (lastdigits[1].Length == 2)
                        {
                            if (Utils.GetRoundoff(objcon.tickprice) == 2)
                            {
                                int lastdigit = Convert.ToInt32(modifyprice.ToString().Substring(modifyprice.ToString().Length - 1, 1));
                                if (lastdigit > 5)
                                {
                                    modifyprice = decimal.Round(Convert.ToDecimal(modifyprice.ToString().Substring(0, modifyprice.ToString().Length - 1)), 2);
                                    modifyprice += 0.1M;
                                }
                                else if (lastdigit < 5 && lastdigit > 0)
                                    modifyprice = Convert.ToDecimal(modifyprice.ToString().Substring(0, modifyprice.ToString().Length - 1) + "5");
                            }
                        }
                    }
                    if (modifyprice != 0)
                    {
                        if (objord.producttype == 1) // RL 
                        {
                            if (objord.buysell == 1) // BUY
                            {
                                if (modifyprice >= objfeeds.ask)
                                {
                                    modifyprice = 0;
                                    //objord.price = objfeeds.ask;
                                }
                            }
                            else // SELL
                            {
                                if (modifyprice <= objfeeds.bid)
                                {
                                    modifyprice = 0;
                                    //objord.price = objfeeds.bid;
                                }
                            }
                        }
                        else if (objord.producttype == 2)// SL
                        {
                            if (objord.buysell == 1) // BUY
                            {
                                if (objfeeds.ask >= modifyprice)
                                {
                                    //return "Invalid SL price found.";
                                    // li.Add("Invalid SL price found");
                                    cm.Message = "Invalid SL price found";
                                    li.Add(new common { Message = cm.Message });
                                    response.data = li;
                                    common.status = "Failed";
                                    common.result = li.Count().ToString() + " record found";
                                    response.description = common;
                                    // JSonResponse js = new JSonResponse();
                                    var object03 = js.JSon(response);
                                    return Request.CreateResponse(HttpStatusCode.OK, object03);
                                }
                            }
                            else // SELL
                            {
                                if (modifyprice >= objfeeds.bid)
                                {
                                    //return "Invalid SL price found.";
                                    //li.Add("Invalid SL price found");
                                    cm.Message = "Invalid SL price found";
                                    li.Add(new common { Message = cm.Message });
                                    response.data = li;
                                    common.status = "Failed";
                                    common.result = li.Count().ToString() + " record found";
                                    response.description = common;
                                    // JSonResponse js = new JSonResponse();
                                    var object03 = js.JSon(response);
                                    return Request.CreateResponse(HttpStatusCode.OK, object03);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (objord.buysell == 1) // BUY
                        {
                            if (modifyprice >= objfeeds.ask)
                                objord.price = objfeeds.ask;
                        }
                        else // SELL
                        {
                            if (modifyprice <= objfeeds.bid)
                                objord.price = objfeeds.bid;
                        }
                    }

                }
                if (objlimits.tradeattributes == 2)
                {
                    if (!isClosePos)
                    {
                        //return "Restricted to Create New Positions, Only allowed to Close Positions by Admin, please contact Admin";
                        // li.Add("Restricted to Create New Positions, Only allowed to Close Positions by Admin, please contact Admin");
                        cm.Message = "Restricted to Create New Positions, Only allowed to Close Positions by Admin, please contact Admin";
                        li.Add(new common { Message = cm.Message });
                        response.data = li;
                        common.status = "Failed";
                        common.result = li.Count().ToString() + " record found";
                        response.description = common;
                        // JSonResponse js = new JSonResponse();
                        var object03 = js.JSon(response);
                        return Request.CreateResponse(HttpStatusCode.OK, object03);
                    }
                    if (symlots < curcnflots)
                    {
                        cm.Message = "Restricted to Create New Positions, Only allowed to Close Positions by Admin, please contact Admin";
                        li.Add(new common { Message = cm.Message });
                        response.data = li;
                        common.status = "Failed";
                        common.result = li.Count().ToString() + " record found";
                        response.description = common;
                        // JSonResponse js = new JSonResponse();
                        var object03 = js.JSon(response);
                        return Request.CreateResponse(HttpStatusCode.OK, object03);
                    }
                }
                string msg = string.Empty;
                if (_utils.PlaceOrder(objord, Conn, objfeeds, ref msg))
                {
                    if (objord.ordstatus == 1)
                    {
                        //return "Order executed Successfully!!";
                        //li.Add("Order executed Successfully");
                        cm.Message = "Order executed Successfully" + "at Price:-" + objord.Ordprice;
                        li.Add(new common { Message = cm.Message });
                        response.data = li;
                        common.status = "Success";
                        common.result = li.Count().ToString() + " record found";
                        response.description = common;
                        // JSonResponse js = new JSonResponse();
                        var object03 = js.JSon(response);
                        return Request.CreateResponse(HttpStatusCode.OK, object03);
                    }
                    else if (objord.ordstatus == 2)
                    {
                        //return "Order placed Successfully!!";
                        //li.Add("Order placed Successfully");
                        cm.Message = "Order placed Successfully" + "at Price:-" + objord.Ordprice;
                        li.Add(new common { Message = cm.Message });
                        response.data = li;
                        common.status = "Success";
                        common.result = li.Count().ToString() + " record found";
                        response.description = common;
                        // JSonResponse js = new JSonResponse();
                        var object03 = js.JSon(response);
                        return Request.CreateResponse(HttpStatusCode.OK, object03);
                    }
                }
                else
                {
                    // li.Add(msg);
                    cm.Message = msg;
                    li.Add(new common { Message = cm.Message });
                    response.data = li;
                    common.status = "Failed";
                    common.result = li.Count().ToString() + " record found";
                    response.description = common;
                    // JSonResponse js = new JSonResponse();
                    var object03 = js.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.OK, object03);
                }
                response.data = li;
                common.status = "Success";
                common.result = li.Count().ToString() + " record found";
                response.description = common;

                var object11 = js.JSon(response);
                return Request.CreateResponse(HttpStatusCode.OK, object11);
            //}
            var object1 = js.JSon(response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }
    }
}