using DTS110Tokenbase.Classfile;
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

namespace DTS110Tokenbase.Controllers
{
    [Authorize]
    public class DefaultController : ApiController
    {
        [Route("api/Default/getSymbols")]
        public HttpResponseMessage getSymbols(string exch)
        {
           
            //NameValueCollection headerList = httpContext.Request.Headers;
            //var authorizationField = headerList.Get("postman-token");
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                common.status = "Failed";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            }
            else
            {
                SqlConnection feedConn = Utils.feedconn;
                if (feedConn.State != ConnectionState.Open)
                {
                    feedConn.Open();
                }
                using (SqlCommand cmd = new SqlCommand("getsymbolsforMobile", feedConn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@exch", exch);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            getSymbols getsym = new getSymbols();
                            if (!reader.IsDBNull(0))
                                getsym.Symbol = reader.GetString(0);
                            li.Add(new getSymbols { exch = exch, Symbol = getsym.Symbol });
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
        [Route("api/Default/getExpiry")]
        public HttpResponseMessage getExpiry(string symbol)
        {
            SqlConnection feedConn = Utils.feedconn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
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

                using (SqlCommand cmd = new SqlCommand("[getsymbolsExpiryforMobile]", feedConn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@symbol", symbol);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            getSymbolExpiry getsym = new getSymbolExpiry();
                            if (!reader.IsDBNull(0))
                                getsym.id = reader.GetInt32(0);
                            if (!reader.IsDBNull(1))
                                getsym.exchange = reader.GetValue(1).ToString();
                            if (!reader.IsDBNull(2))
                                getsym.symbol = reader.GetString(2);
                            if (!reader.IsDBNull(2))
                                getsym.expiry = reader.GetString(2).Split(' ')[1];
                            li.Add(new getSymbolExpiry { id = getsym.id, exchange = getsym.exchange, symbol = getsym.symbol, expiry = getsym.expiry });
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
        [Route("api/Default/getToken")]
        public HttpResponseMessage getToken(string exchange,string symbol,string expiry)
        {
            SqlConnection feedConn = Utils.feedconn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
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

                using (SqlCommand cmd = new SqlCommand("[getTokenforMobile]", feedConn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@symbol", symbol);
                    cmd.Parameters.AddWithValue("@exhange", exchange);
                    cmd.Parameters.AddWithValue("@expiry", expiry);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            getToken getsym = new getToken();
                            //if (!reader.IsDBNull(0))
                            //    getsym.id = reader.GetInt32(0);
                            //if (!reader.IsDBNull(1))
                            //    getsym.exchange = reader.GetValue(1).ToString();
                            if (!reader.IsDBNull(0))
                                getsym.symbol = reader.GetString(0);
                            //if (!reader.IsDBNull(2))
                            //    getsym.expiry = reader.GetString(2).Split(' ')[1];
                            li.Add(new getToken { symbol = getsym.symbol });
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

        [Route("api/Default/getDashboard")]
        public HttpResponseMessage getDashboard(string from, string to)
        {
            SqlConnection feedConn = Utils.feedconn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
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
                string symbols = string.Empty;

                using (SqlCommand cmd = new SqlCommand("[getDefaultSymbols]", feedConn) { CommandType = CommandType.StoredProcedure })
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int i = 0;
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                if (i == 0)
                                    symbols += reader.GetString(0);
                                else
                                    symbols += ',' + reader.GetString(0);
                            }
                            i++;
                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand("GetDashboard_Test", feedConn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@symbols", symbols);
                    cmd.Parameters.AddWithValue("@from", from);
                    cmd.Parameters.AddWithValue("@to", to);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            DateTime dt = reader.GetDateTime(15);
                            DateTime exp = reader.GetDateTime(16);
                            dashboard ds = new dashboard();

                            if (!reader.IsDBNull(0))
                                ds.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                ds.bid = reader.GetDouble(1);

                            if (!reader.IsDBNull(2))
                                ds.bidqty = reader.GetInt32(2);

                            if (!reader.IsDBNull(3))
                                ds.ask = reader.GetDouble(3);

                            if (!reader.IsDBNull(4))
                                ds.askqty = reader.GetInt32(4);

                            if (!reader.IsDBNull(5))
                                ds.ltp = reader.GetDouble(5);

                            if (!reader.IsDBNull(6))
                                ds.open = reader.GetDouble(6);

                            if (!reader.IsDBNull(7))
                                ds.close = reader.GetDouble(7);

                            if (!reader.IsDBNull(8))
                                ds.high = reader.GetDouble(8);

                            if (!reader.IsDBNull(9))
                                ds.low = reader.GetDouble(9);

                            if (!reader.IsDBNull(10))
                                ds.vol = reader.GetInt64(10);

                            if (!reader.IsDBNull(11))
                                ds.oi = reader.GetInt64(11);

                            if (!reader.IsDBNull(12))
                                ds.change = reader.GetDouble(12);

                            if (!reader.IsDBNull(13))
                                ds.netchange = reader.GetDouble(13);

                            if (!reader.IsDBNull(14))
                                ds.lotsize = reader.GetInt32(14);


                            if (!reader.IsDBNull(18))
                                ds.exchange = reader.GetString(18);
                            li.Add(new dashboard { symbol = ds.symbol, bid = ds.bid, bidqty = ds.bidqty, ask = ds.ask, askqty = ds.askqty, ltp = ds.ltp, open = ds.open, high = ds.high, low = ds.low, close = ds.close, vol = ds.vol, oi = ds.oi, change = ds.change, netchange = ds.netchange, lotsize = ds.lotsize, ltt = dt.ToString("hh:mm:ss"), lut = DateTime.Now.ToString("hh:mm:ss"), expiry = exp.ToString("dd/MMM/yyyy"), exchange = ds.exchange.Trim() });
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

        [Route("api/Default/getDashboardT")]
        public HttpResponseMessage getDashboardT(string token,string from, string to)
        {
            SqlConnection feedConn = Utils.feedconn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
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

                using (SqlCommand cmd = new SqlCommand("GetDashboard_Test", feedConn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@symbols", token);
                    cmd.Parameters.AddWithValue("@from", from);
                    cmd.Parameters.AddWithValue("@to", to);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            DateTime dt = reader.GetDateTime(15);
                            DateTime exp = reader.GetDateTime(16);
                            dashboard ds = new dashboard();

                            if (!reader.IsDBNull(0))
                                ds.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                ds.bid = reader.GetDouble(1);

                            if (!reader.IsDBNull(2))
                                ds.bidqty = reader.GetInt32(2);

                            if (!reader.IsDBNull(3))
                                ds.ask = reader.GetDouble(3);

                            if (!reader.IsDBNull(4))
                                ds.askqty = reader.GetInt32(4);

                            if (!reader.IsDBNull(5))
                                ds.ltp = reader.GetDouble(5);

                            if (!reader.IsDBNull(6))
                                ds.open = reader.GetDouble(6);

                            if (!reader.IsDBNull(7))
                                ds.close = reader.GetDouble(7);

                            if (!reader.IsDBNull(8))
                                ds.high = reader.GetDouble(8);

                            if (!reader.IsDBNull(9))
                                ds.low = reader.GetDouble(9);

                            if (!reader.IsDBNull(10))
                                ds.vol = reader.GetInt64(10);

                            if (!reader.IsDBNull(11))
                                ds.oi = reader.GetInt64(11);

                            if (!reader.IsDBNull(12))
                                ds.change = reader.GetDouble(12);

                            if (!reader.IsDBNull(13))
                                ds.netchange = reader.GetDouble(13);

                            if (!reader.IsDBNull(14))
                                ds.lotsize = reader.GetInt32(14);


                            if (!reader.IsDBNull(18))
                                ds.exchange = reader.GetString(18);
                            li.Add(new dashboard { symbol = ds.symbol, bid = ds.bid, bidqty = ds.bidqty, ask = ds.ask, askqty = ds.askqty, ltp = ds.ltp, open = ds.open, high = ds.high, low = ds.low, close = ds.close, vol = ds.vol, oi = ds.oi, change = ds.change, netchange = ds.netchange, lotsize = ds.lotsize, ltt = dt.ToString("hh:mm:ss"), lut = DateTime.Now.ToString("hh:mm:ss"), expiry = exp.ToString("dd/MMM/yyyy"), exchange = ds.exchange.Trim() });
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
        public string FormatDecimal(decimal val)
        {
            decimal newValue = ((Int64)(val * 100)) / 100m;
            return newValue.ToString("N2");
        }
        [Route("api/Default/getChangePassword")]
        [HttpPost]
        public HttpResponseMessage getChangePassword([FromBody]password ps)
        {
            Utils _utils = new Utils();
            SqlConnection Conn = Utils.conn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                common.status = "Failed";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            }
            else
            {
                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("UpdateUserPasswordForMobile", Conn) { CommandType = CommandType.StoredProcedure })
                {

                    string _oldpass = _utils.Encryptdata(ps.oldpass);
                    string _newpass = _utils.Encryptdata(ps.newpass);
                    cmd.Parameters.AddWithValue("@username", ps.username);
                    cmd.Parameters.AddWithValue("@oldPassword", _oldpass);
                    cmd.Parameters.AddWithValue("@newpassword", _newpass);
                    int res = cmd.ExecuteNonQuery();
                    common cm = new common();
                    if (res > 0)
                    {
                        cm.Message = "Password Change Successfully";
                    }
                    else
                    {
                        cm.Message = "Password Change Unsuccessfully";
                    }
                    li.Add(new common { Message = cm.Message });
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
        [Route("api/Default/topGainers")]
        [HttpGet]
        public HttpResponseMessage topGainers(string Exchange)
        {
            Utils _utils = new Utils();
            SqlConnection FeedConn = Utils.feedconn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                common.status = "Failed";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            }
            else
            {
                if (FeedConn.State != ConnectionState.Open)
                {
                    FeedConn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("TopGainers", FeedConn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exchange);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            topgainerlooser tp = new topgainerlooser();
                            if (!reader.IsDBNull(0))
                                tp.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(2))
                                tp.Ltp = FormatDecimal((decimal)Math.Round(Convert.ToDouble(reader.GetDouble(2)), 2));

                            if (!reader.IsDBNull(3))
                                tp.Change = FormatDecimal((decimal)Math.Round(Convert.ToDouble(reader.GetDouble(3)), 2));
                            //result = result + "{\"message\": \"success\",\"symbol\": \"" + reader.GetString(0) + "\",\"ltp\": \"" + FormatDecimal((decimal)Math.Round(Convert.ToDouble(reader.GetDouble(2)), 2)) + "\",\"change\": \"" + FormatDecimal((decimal)Math.Round(Convert.ToDouble(reader.GetDouble(3)), 2)) + "\"}";
                            li.Add(new topgainerlooser { symbol = tp.symbol, Ltp = tp.Ltp, Change = tp.Change });
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

        [Route("api/Default/topLosers")]
        [HttpGet]
        public HttpResponseMessage topLosers(string Exchange)
        {
            Utils _utils = new Utils();
            SqlConnection FeedConn = Utils.feedconn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                common.status = "Failed";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            }
            else
            {
                if (FeedConn.State != ConnectionState.Open)
                {
                    FeedConn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("TopLoosers", FeedConn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exchange);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            topgainerlooser tp = new topgainerlooser();
                            if (!reader.IsDBNull(0))
                                tp.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(2))
                                tp.Ltp = FormatDecimal((decimal)Math.Round(Convert.ToDouble(reader.GetDouble(2)), 2));

                            if (!reader.IsDBNull(3))
                                tp.Change = FormatDecimal((decimal)Math.Round(Convert.ToDouble(reader.GetDouble(3)), 2));
                            //result = result + "{\"message\": \"success\",\"symbol\": \"" + reader.GetString(0) + "\",\"ltp\": \"" + FormatDecimal((decimal)Math.Round(Convert.ToDouble(reader.GetDouble(2)), 2)) + "\",\"change\": \"" + FormatDecimal((decimal)Math.Round(Convert.ToDouble(reader.GetDouble(3)), 2)) + "\"}";
                            li.Add(new topgainerlooser { symbol = tp.symbol, Ltp = tp.Ltp, Change = tp.Change });
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

        [Route("api/Default/volumeShockers")]
        [HttpGet]
        public HttpResponseMessage volumeShockers(string Exchange)
        {
            Utils _utils = new Utils();
            SqlConnection FeedConn = Utils.feedconn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                common.status = "Failed";
                common.result = li.Count().ToString() + " record found";
                response.description = common;
            }
            else
            {
                if (FeedConn.State != ConnectionState.Open)
                {
                    FeedConn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("TopVolume", FeedConn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exchange);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            volumeShockers tp = new volumeShockers();
                            if (!reader.IsDBNull(0))
                                tp.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                tp.volume = Convert.ToString(reader.GetInt64(1));


                            li.Add(new volumeShockers { symbol = tp.symbol, volume = tp.volume });
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

        [Route("api/Default/setAlert")]
        [HttpPost]
        public HttpResponseMessage setAlert([FromBody]Alert al)
        {
            Utils _utils = new Utils();
            SqlConnection FeedConn = Utils.feedconn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
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
                if (identity.Name != al.clientcode)
                {
                    li.Add("Unauthorized Access for Clientcode= " + al.clientcode);
                    response.data = li;
                    JSonResponse js1 = new JSonResponse();
                    var object11 = js1.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, object11);
                }
                if (FeedConn.State != ConnectionState.Open)
                {
                    FeedConn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("SaveAlerts", FeedConn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@clientcode", al.clientcode);
                    cmd.Parameters.AddWithValue("@condition", al.condtion);
                    cmd.Parameters.AddWithValue("@symbol", al.symbol);
                    cmd.Parameters.AddWithValue("@price", al.price);
                    int res = cmd.ExecuteNonQuery();
                    common cm = new common();
                    if (res > 0)
                        cm.Message = "Alert Set Successfully";
                    else
                        cm.Message = "Alert Set Unsuccessfully";
                    li.Add(new common { Message = cm.Message });
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

        [Route("api/Default/getAlerts")]
        [HttpGet]
        public HttpResponseMessage getAlerts(string clientcode,string status)
        {
            Utils _utils = new Utils();
            SqlConnection FeedConn = Utils.feedconn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
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
                    li.Add("Unauthorized Access for Clientcode= " + clientcode);
                    response.data = li;
                    JSonResponse js1 = new JSonResponse();
                    var object11 = js1.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, object11);
                }
                if (FeedConn.State != ConnectionState.Open)
                {
                    FeedConn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("[getAlert]", FeedConn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@clientcode", clientcode);
                    cmd.Parameters.AddWithValue("@status", status);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            Alert tp = new Alert();

                            if (!reader.IsDBNull(0))
                                tp.id = reader.GetInt32(0).ToString();
                            if (!reader.IsDBNull(2))
                                tp.symbol = reader.GetString(2);
                            if (!reader.IsDBNull(4))
                                tp.price = reader.GetValue(4).ToString();
                            if (reader.GetInt32(3) == 1)
                                tp.condtion = "<";
                            else if (reader.GetInt32(3) == 2)
                                tp.condtion = ">";
                            else
                                tp.condtion = "=";

                            DateTime timestamp;
                            if (status.Equals("0"))
                                timestamp = Convert.ToDateTime(reader.GetValue(5));
                            else
                                timestamp = Convert.ToDateTime(reader.GetValue(5));
                            li.Add(new Alert { id = tp.id, symbol = tp.symbol, condtion = tp.condtion, price = tp.price, timestamp = timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff") });
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

        [Route("api/Default/editAlert")]
        [HttpPost]
        public HttpResponseMessage editAlert([FromBody] Alert al)
        {
            Utils _utils = new Utils();
            SqlConnection FeedConn = Utils.feedconn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
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
                if (identity.Name != al.clientcode)
                {
                    li.Add("Unauthorized Access for Clientcode= " + al.clientcode);
                    response.data = li;
                    JSonResponse js1 = new JSonResponse();
                    var object11 = js1.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, object11);
                }
                if (FeedConn.State != ConnectionState.Open)
                {
                    FeedConn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("SaveAlerts", FeedConn) { CommandType = CommandType.StoredProcedure })
                {
                    common cm = new common();
                    cmd.Parameters.AddWithValue("@id", al.id);
                    cmd.Parameters.AddWithValue("@clientcode", al.clientcode);
                    cmd.Parameters.AddWithValue("@condition", al.condtion);
                    cmd.Parameters.AddWithValue("@symbol", al.symbol);
                    cmd.Parameters.AddWithValue("@price", al.price);
                    int res = cmd.ExecuteNonQuery();
                    if (res > 0)
                        cm.Message = "Alert Update Successfully";
                    else
                        cm.Message = "Alert Update Unsuccessfully";
                    li.Add(new common { Message = cm.Message });
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


        [Route("api/Default/removeAlert")]
        [HttpDelete]
        public HttpResponseMessage removeAlert([FromBody] Alert al)
        {
            Utils _utils = new Utils();
            SqlConnection FeedConn = Utils.feedconn;
            FRatesResponse response = new FRatesResponse();
            CommonResponse common = new CommonResponse();
            List<dynamic> li = new List<dynamic>();
            common.status = "Failed";
            common.result = li.Count().ToString() + " record found";
            response.description = common;
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
                if (identity.Name != al.clientcode)
                {
                    li.Add("Unauthorized Access for Clientcode= " + al.clientcode);
                    response.data = li;
                    JSonResponse js1 = new JSonResponse();
                    var object11 = js1.JSon(response);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, object11);
                }
                if (FeedConn.State != ConnectionState.Open)
                {
                    FeedConn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("[DeleteAlerts]", FeedConn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@id", al.id);
                    cmd.Parameters.AddWithValue("@clientcode", al.clientcode);

                    int res = cmd.ExecuteNonQuery();
                    common cm = new common();
                    if (res > 0)
                        cm.Message = "Alert delete Successfully";
                    else
                        cm.Message = "Alert delete Unsuccessfully";
                    li.Add(new common { Message = cm.Message });
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
    }
}
