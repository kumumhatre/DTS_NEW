using ssmarketnewTokenBase.ClassFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ssmarketnewTokenBase.Controllers
{
    [Authorize]
    public class FII_DII_ActivityController : ApiController
    {
        [HttpGet]
        [Route("api/FII_DII_Activity/New_High_New_Low")]
        public HttpResponseMessage New_High_New_Low(string exchh, int type, string timestamp)
        {
            SqlConnection Conn = Utils.oldfeedconn;
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
                string exchhh = string.Empty;

                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();
                }
                using (SqlCommand cmd = new SqlCommand("newhigh_newlow", Conn) { CommandType = CommandType.StoredProcedure })
                {

                    cmd.Parameters.AddWithValue("@timestamp", timestamp);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            newhighnewlow newhilo = new newhighnewlow();
                            int exch = reader.GetInt32(4);
                            if (exch == 1)
                                exchhh = "MCX";
                            if (exch == 2)
                                exchhh = "NSEFUT";
                            if (exch == 3)
                                exchhh = "NCDEX";
                            if (exch == 4)
                                exchhh = "NSECURR";
                            if (exchh.ToUpper().Contains(exchhh))
                            {

                                if (!reader.IsDBNull(0))
                                    newhilo.Symbol = reader.GetString(0);

                                if (!reader.IsDBNull(1))
                                    newhilo.price = reader.GetValue(1).ToString();

                                if (!reader.IsDBNull(2))
                                    newhilo.highlow = Convert.ToInt32(reader.GetValue(2));

                                if (!reader.IsDBNull(3))
                                    newhilo.Timestamp = Convert.ToDateTime(reader.GetValue(3));


                                if (type == 0)
                                {
                                    li.Add(new newhighnewlow { Symbol = newhilo.Symbol, price = newhilo.price, highlow = newhilo.highlow, Timestamp = newhilo.Timestamp });
                                    //result = result + "{\"message\":\"success\", \"Symbol\": \"" + newhilo.Symbol + "\", \"price\": \"" + newhilo.price + "\", \"highlow\": \"" + newhilo.highlow + "\", \"Timestamp\": \"" + newhilo.Timestamp + "\"}";

                                }

                                else if (type == newhilo.highlow)
                                {
                                    li.Add(new newhighnewlow { Symbol = newhilo.Symbol, price = newhilo.price, highlow = newhilo.highlow, Timestamp = newhilo.Timestamp });
                                    //result = result + "{\"message\":\"success\", \"Symbol\": \"" + newhilo.Symbol + "\", \"price\": \"" + newhilo.price + "\", \"highlow\": \"" + newhilo.highlow + "\", \"Timestamp\": \"" + newhilo.Timestamp + "\"}";

                                }

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

        [HttpGet]
        [Route("api/FII_DII_Activity/Real_Time_Pivots")]
        public HttpResponseMessage Real_Time_Pivots(string timestamp)
        {
            SqlConnection Conn = Utils.oldfeedconn;
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
                string exchhh = string.Empty;

                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();
                }


                using (SqlCommand cmd = new SqlCommand("RealTimePivots", Conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@timestamp", timestamp);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            realtimepivots rp = new realtimepivots();
                            if (!reader.IsDBNull(0))
                                rp.pivotname = reader.GetString(0);
                            if (!reader.IsDBNull(1))
                                rp.levels = reader.GetString(1);
                            if (!reader.IsDBNull(2))
                                rp.symbol = reader.GetString(2);
                            if (!reader.IsDBNull(3))
                                rp.price = reader.GetValue(3).ToString();
                            if (!reader.IsDBNull(4))
                                rp.Timestamp = Convert.ToDateTime(reader.GetValue(4));

                            li.Add(new realtimepivots { pivotname = rp.pivotname, levels = rp.levels, symbol = rp.symbol, price = rp.price, Timestamp = rp.Timestamp });


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

        [HttpGet]
        [Route("api/FII_DII_Activity/daily_Pivots")]
        public HttpResponseMessage daily_Pivots(string symbol, string type)
        {
            SqlConnection Conn = Utils.oldfeedconn;
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
                string levels = string.Empty;

                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("dailyPivots", Conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@symbol", symbol);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pivots _pi = new pivots();
                            li.Clear();

                            if (!reader.IsDBNull(1))
                                levels += reader.GetValue(1).ToString() + ",";
                            _pi.LEVEL = levels;
                            if (!reader.IsDBNull(3))
                                _pi.R3 = reader.GetValue(3).ToString();
                            if (!reader.IsDBNull(4))
                                _pi.R2 = reader.GetValue(4).ToString();
                            if (!reader.IsDBNull(5))
                                _pi.R1 = reader.GetValue(5).ToString();
                            if (!reader.IsDBNull(6))
                                _pi.PP = reader.GetValue(6).ToString();
                            if (!reader.IsDBNull(7))
                                _pi.S1 = reader.GetValue(7).ToString();
                            if (!reader.IsDBNull(8))
                                _pi.S2 = reader.GetValue(8).ToString();
                            if (!reader.IsDBNull(9))
                                _pi.S3 = reader.GetValue(9).ToString();



                            li.Add(new pivots { LEVEL = _pi.LEVEL, R3 = _pi.R3, R2 = _pi.R2, R1 = _pi.R1, PP = _pi.PP, S1 = _pi.S1, S2 = _pi.S2, S3 = _pi.S3 });
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

        [HttpGet]
        [Route("api/FII_DII_Activity/weekly_Pivots")]
        public HttpResponseMessage weekly_Pivots(string symbol, string type)
        {
            SqlConnection Conn = Utils.oldfeedconn;
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
                string levels = string.Empty;

                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();

                }

                using (SqlCommand cmd = new SqlCommand("weeklyPivots", Conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@symbol", symbol);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss"));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pivots _pi = new pivots();
                            li.Clear();

                            if (!reader.IsDBNull(1))
                                levels += reader.GetValue(1).ToString() + ",";
                            _pi.LEVEL = levels;
                            if (!reader.IsDBNull(3))
                                _pi.R3 = reader.GetValue(3).ToString();
                            if (!reader.IsDBNull(4))
                                _pi.R2 = reader.GetValue(4).ToString();
                            if (!reader.IsDBNull(5))
                                _pi.R1 = reader.GetValue(5).ToString();
                            if (!reader.IsDBNull(6))
                                _pi.PP = reader.GetValue(6).ToString();
                            if (!reader.IsDBNull(7))
                                _pi.S1 = reader.GetValue(7).ToString();
                            if (!reader.IsDBNull(8))
                                _pi.S2 = reader.GetValue(8).ToString();
                            if (!reader.IsDBNull(9))
                                _pi.S3 = reader.GetValue(9).ToString();



                            li.Add(new pivots { LEVEL = _pi.LEVEL, R3 = _pi.R3, R2 = _pi.R2, R1 = _pi.R1, PP = _pi.PP, S1 = _pi.S1, S2 = _pi.S2, S3 = _pi.S3 });
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

        [HttpGet]
        [Route("api/FII_DII_Activity/fii_dii_Overview")]
        public HttpResponseMessage fii_dii_Overview(string validity)
        {
            SqlConnection Conn = Utils.fivestarconn;
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
                string levels = string.Empty;

                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();

                }
                fii_dii_Overview fiidii = new fii_dii_Overview();
                string date = string.Empty;
                DateTime d1 = DateTime.Now;
                if (validity.ToUpper() == "DAILY")
                {
                    using (SqlCommand cmd = new SqlCommand("getEQUITY_FII_DII_daily", Conn) { CommandType = CommandType.StoredProcedure })
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    d1 = Convert.ToDateTime(reader.GetValue(0));
                                date = d1.ToString("yyyy-MM-dd 00:00:00.000");
                                fiidii.DATE = date;
                                if (!reader.IsDBNull(1))
                                    fiidii.fiicash = Math.Round(Convert.ToDecimal(reader.GetValue(1)));
                                if (!reader.IsDBNull(2))
                                    fiidii.diicash = Math.Round(Convert.ToDecimal(reader.GetValue(2)));
                                decimal futindxnetpurches = 0.0M;
                                decimal optindxnetpurches = 0.0M;
                                decimal futstocknetpurches = 0.0M;
                                decimal optstocknetpurches = 0.0M;

                                using (SqlCommand cmd1 = new SqlCommand("getFII_INDEX_daily", Conn) { CommandType = CommandType.StoredProcedure })
                                {
                                    cmd1.Parameters.AddWithValue("@date", date);
                                    using (var reader1 = cmd1.ExecuteReader())
                                    {
                                        while (reader1.Read())
                                        {
                                            if (!reader1.IsDBNull(0))
                                                futindxnetpurches = Convert.ToDecimal(reader1.GetValue(0));
                                            if (!reader1.IsDBNull(1))
                                                optindxnetpurches = Convert.ToDecimal(reader1.GetValue(1));

                                        }

                                    }
                                }
                                using (SqlCommand cmd2 = new SqlCommand("getFII_Stock_daily", Conn) { CommandType = CommandType.StoredProcedure })
                                {
                                    cmd2.Parameters.AddWithValue("@date", date);
                                    using (var reader2 = cmd2.ExecuteReader())
                                    {
                                        while (reader2.Read())
                                        {
                                            if (!reader2.IsDBNull(0))
                                                futstocknetpurches = Convert.ToDecimal(reader2.GetValue(0));
                                            if (!reader2.IsDBNull(1))
                                                optstocknetpurches = Convert.ToDecimal(reader2.GetValue(1));

                                        }

                                    }
                                }
                                fiidii.fiiFut = Math.Round(futindxnetpurches + futstocknetpurches);
                                fiidii.fiiOpt = Math.Round(optindxnetpurches + optstocknetpurches);


                                li.Add(new fii_dii_Overview { DATE = fiidii.DATE, fiicash = fiidii.fiicash, diicash = fiidii.diicash, fiiFut = fiidii.fiiFut, fiiOpt = fiidii.fiiOpt });
                            }
                        }
                    }
                }


                if (validity.ToUpper() == "WEEKLY")
                {
                    DateTime dt = DateTime.Now;
                    int day = dt.Month;
                    //int i = 0;
                    string EndDATE = string.Empty;
                    string StartDATE = string.Empty;
                    decimal futindxnetpurches = 0.0M;
                    decimal optindxnetpurches = 0.0M;
                    decimal futstocknetpurches = 0.0M;
                    decimal optstocknetpurches = 0.0M;
                    for (int i = 0; i < 12; i++)
                    {
                        if (i > 0)
                            dt = dt.AddDays(-7);
                        if (dt.DayOfWeek.ToString() == "Monday")
                        {
                            StartDATE = dt.ToString("yyyy-MM-dd 00:00:00.000");
                            EndDATE = dt.AddDays(5).ToString("yyyy-MM-dd 00:00:00.000");
                        }
                        else if (dt.DayOfWeek.ToString() == "Tuesday")
                        {

                            StartDATE = dt.AddDays(-1).ToString("yyyy-MM-dd 00:00:00.000");
                            EndDATE = dt.AddDays(4).ToString("yyyy-MM-dd 00:00:00.000");
                        }
                        else if (dt.DayOfWeek.ToString() == "Wednesday")
                        {

                            StartDATE = dt.AddDays(-2).ToString("yyyy-MM-dd 00:00:00.000");
                            EndDATE = dt.AddDays(3).ToString("yyyy-MM-dd 00:00:00.000");
                        }
                        else if (dt.DayOfWeek.ToString() == "Thursday")
                        {

                            StartDATE = dt.AddDays(-3).ToString("yyyy-MM-dd 00:00:00.000");
                            EndDATE = dt.AddDays(2).ToString("yyyy-MM-dd 00:00:00.000");
                        }
                        else if (dt.DayOfWeek.ToString() == "Friday")
                        {

                            StartDATE = dt.AddDays(-4).ToString("yyyy-MM-dd 00:00:00.000");
                            EndDATE = dt.AddDays(1).ToString("yyyy-MM-dd 00:00:00.000");
                        }
                        else if (dt.DayOfWeek.ToString() == "Saturday")
                        {

                            StartDATE = dt.AddDays(-5).ToString("yyyy-MM-dd 00:00:00.000");
                            EndDATE = dt.AddDays(0).ToString("yyyy-MM-dd 00:00:00.000");
                        }


                        using (SqlCommand cmd = new SqlCommand("getEQUITY_FII_DII_weekly_monthly", Conn) { CommandType = CommandType.StoredProcedure })
                        {
                            cmd.Parameters.AddWithValue("@startdate", StartDATE);
                            cmd.Parameters.AddWithValue("@enddate", EndDATE);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    //if (!reader.IsDBNull(0))
                                    //   fiidii.DATE = Convert.ToDateTime(reader.GetValue(0));
                                    //date = fiidii.DATE.ToString("yyyy-MM-dd 00:00:00.000");
                                    fiidii.DATE = StartDATE + " TO " + EndDATE;
                                    if (!reader.IsDBNull(0))
                                        fiidii.fiicash = Math.Round(Convert.ToDecimal(reader.GetValue(0)));
                                    if (!reader.IsDBNull(1))
                                        fiidii.diicash = Math.Round(Convert.ToDecimal(reader.GetValue(1)));


                                    using (SqlCommand cmd1 = new SqlCommand("getFII_INDEX_weekly_monthly", Conn) { CommandType = CommandType.StoredProcedure })
                                    {
                                        cmd1.Parameters.AddWithValue("@startdate", StartDATE);
                                        cmd1.Parameters.AddWithValue("@enddate", EndDATE);
                                        using (var reader1 = cmd1.ExecuteReader())
                                        {
                                            while (reader1.Read())
                                            {
                                                if (!reader1.IsDBNull(0))
                                                    futindxnetpurches = Convert.ToDecimal(reader1.GetValue(0));
                                                if (!reader1.IsDBNull(1))
                                                    optindxnetpurches = Convert.ToDecimal(reader1.GetValue(1));

                                            }

                                        }


                                    }

                                    using (SqlCommand cmd2 = new SqlCommand("getFII_Stock_weekly_monthly", Conn) { CommandType = CommandType.StoredProcedure })
                                    {
                                        cmd2.Parameters.AddWithValue("@startdate", StartDATE);
                                        cmd2.Parameters.AddWithValue("@enddate", EndDATE);
                                        using (var reader2 = cmd2.ExecuteReader())
                                        {
                                            while (reader2.Read())
                                            {
                                                if (!reader2.IsDBNull(0))
                                                    futstocknetpurches = Convert.ToDecimal(reader2.GetValue(0));
                                                if (!reader2.IsDBNull(1))
                                                    optstocknetpurches = Convert.ToDecimal(reader2.GetValue(1));

                                            }

                                        }
                                    }
                                    fiidii.fiiFut = Math.Round(futindxnetpurches + futstocknetpurches);
                                    fiidii.fiiOpt = Math.Round(optindxnetpurches + optstocknetpurches);
                                    // i++;
                                    DateTime dt1 = Convert.ToDateTime(EndDATE);
                                    li.Add(new fii_dii_Overview { DATE = fiidii.DATE, fiicash = fiidii.fiicash, diicash = fiidii.diicash, fiiFut = fiidii.fiiFut, fiiOpt = fiidii.fiiOpt });
                                    fiidii.fiicash = 0.0M;
                                    fiidii.diicash = 0.0M;
                                    fiidii.fiiFut = 0.0M;
                                    fiidii.fiiOpt = 0.0M;
                                    futindxnetpurches = 0.0M;
                                    optindxnetpurches = 0.0M;
                                    futstocknetpurches = 0.0M;
                                    optstocknetpurches = 0.0M;
                                }
                            }
                        }
                    }
                }

                if (validity.ToUpper() == "MONTHLY")
                {
                    DateTime dt = DateTime.Now;
                    int day = dt.Month;
                    string EndDATE = string.Empty;
                    string StartDATE = string.Empty;
                    decimal futindxnetpurches = 0.0M;
                    decimal optindxnetpurches = 0.0M;
                    decimal futstocknetpurches = 0.0M;
                    decimal optstocknetpurches = 0.0M;
                    for (int i = 0; i < 12; i++)
                    {
                        if (i > 0)
                        {
                            dt = dt.AddDays(-30);
                            day = dt.Month;
                        }
                        if (day == 1)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 00:00:00.000");
                        }
                        else if (day == 2)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-28 00:00:00.000");
                        }
                        else if (day == 3)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 00:00:00.000");
                        }
                        else if (day == 4)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-30 00:00:00.000");
                        }
                        else if (day == 5)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 00:00:00.000");
                        }
                        else if (day == 6)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-30 00:00:00.000");
                        }
                        else if (day == 7)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 00:00:00.000");
                        }
                        else if (day == 8)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 23:59:59.000");
                        }
                        else if (day == 9)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-30 00:00:00.000");
                        }
                        else if (day == 10)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 00:00:00.000");
                        }
                        else if (day == 11)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-30 00:00:00.000");
                        }
                        else if (day == 12)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 00:00:00.000");
                        }

                        using (SqlCommand cmd = new SqlCommand("getEQUITY_FII_DII_weekly_monthly", Conn) { CommandType = CommandType.StoredProcedure })
                        {
                            cmd.Parameters.AddWithValue("@startdate", StartDATE);
                            cmd.Parameters.AddWithValue("@enddate", EndDATE);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    //if (!reader.IsDBNull(0))
                                    //   fiidii.DATE = Convert.ToDateTime(reader.GetValue(0));
                                    //date = fiidii.DATE.ToString("yyyy-MM-dd 00:00:00.000");
                                    fiidii.DATE = StartDATE + " TO " + EndDATE;
                                    if (!reader.IsDBNull(0))
                                        fiidii.fiicash = Math.Round(Convert.ToDecimal(reader.GetValue(0)));
                                    if (!reader.IsDBNull(1))
                                        fiidii.diicash = Math.Round(Convert.ToDecimal(reader.GetValue(1)));

                                    using (SqlCommand cmd1 = new SqlCommand("getFII_INDEX_weekly_monthly", Conn) { CommandType = CommandType.StoredProcedure })
                                    {
                                        cmd1.Parameters.AddWithValue("@startdate", StartDATE);
                                        cmd1.Parameters.AddWithValue("@enddate", EndDATE);
                                        using (var reader1 = cmd1.ExecuteReader())
                                        {
                                            while (reader1.Read())
                                            {
                                                if (!reader1.IsDBNull(0))
                                                    futindxnetpurches = Convert.ToDecimal(reader1.GetValue(0));
                                                if (!reader1.IsDBNull(1))
                                                    optindxnetpurches = Convert.ToDecimal(reader1.GetValue(1));

                                            }

                                        }
                                    }
                                    using (SqlCommand cmd2 = new SqlCommand("getFII_Stock_weekly_monthly", Conn) { CommandType = CommandType.StoredProcedure })
                                    {
                                        cmd2.Parameters.AddWithValue("@startdate", StartDATE);
                                        cmd2.Parameters.AddWithValue("@enddate", EndDATE);
                                        using (var reader2 = cmd2.ExecuteReader())
                                        {
                                            while (reader2.Read())
                                            {
                                                if (!reader2.IsDBNull(0))
                                                    futstocknetpurches = Convert.ToDecimal(reader2.GetValue(0));
                                                if (!reader2.IsDBNull(1))
                                                    optstocknetpurches = Convert.ToDecimal(reader2.GetValue(1));

                                            }

                                        }
                                    }
                                    fiidii.fiiFut = Math.Round(futindxnetpurches + futstocknetpurches);
                                    fiidii.fiiOpt = Math.Round(optindxnetpurches + optstocknetpurches);
                                    // i++;
                                    DateTime dt1 = Convert.ToDateTime(EndDATE);
                                    li.Add(new fii_dii_Overview { DATE = fiidii.DATE, fiicash = fiidii.fiicash, diicash = fiidii.diicash, fiiFut = fiidii.fiiFut, fiiOpt = fiidii.fiiOpt });
                                    fiidii.fiicash = 0.0M;
                                    fiidii.diicash = 0.0M;
                                    fiidii.fiiFut = 0.0M;
                                    fiidii.fiiOpt = 0.0M;
                                    futindxnetpurches = 0.0M;
                                    optindxnetpurches = 0.0M;
                                    futstocknetpurches = 0.0M;
                                    optstocknetpurches = 0.0M;
                                }
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

        [HttpGet]
        [Route("api/FII_DII_Activity/fii_dii_Single")]
        public HttpResponseMessage fii_dii_Single(string validity)
        {
            SqlConnection Conn = Utils.fivestarconn;
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
                string levels = string.Empty;

                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();

                }
                fii_dii_single fiidii = new fii_dii_single();
                string date = string.Empty;
                DateTime d1 = DateTime.Now;
                if (validity.ToUpper() == "DAILY")
                {
                    using (SqlCommand cmd = new SqlCommand("getFII_INDEX_single_daily", Conn) { CommandType = CommandType.StoredProcedure })
                    {

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    d1 = Convert.ToDateTime(reader.GetValue(0));
                                date = d1.ToString("yyyy-MM-dd 00:00:00.000");
                                fiidii.DATE = date;
                                if (!reader.IsDBNull(1))
                                    fiidii.futidx = Math.Round(Convert.ToDecimal(reader.GetValue(1)));
                                if (!reader.IsDBNull(2))
                                    fiidii.optidx = Math.Round(Convert.ToDecimal(reader.GetValue(2)));

                                using (SqlCommand cmd1 = new SqlCommand("getFII_STOCK_single_daily", Conn) { CommandType = CommandType.StoredProcedure })
                                {
                                    cmd1.Parameters.AddWithValue("@date", date);
                                    using (var reader1 = cmd1.ExecuteReader())
                                    {
                                        while (reader1.Read())
                                        {
                                            if (!reader1.IsDBNull(0))
                                                fiidii.futstk = Math.Round(Convert.ToDecimal(reader1.GetValue(0)));
                                            if (!reader1.IsDBNull(1))
                                                fiidii.optstk = Math.Round(Convert.ToDecimal(reader1.GetValue(1)));
                                        }
                                    }
                                }
                                li.Add(new fii_dii_single { DATE = fiidii.DATE, futidx = fiidii.futidx, optidx = fiidii.optidx, futstk = fiidii.futstk, optstk = fiidii.optstk });

                            }
                        }
                    }
                }

                if (validity.ToUpper() == "WEEKLY")
                {

                    DateTime dt = DateTime.Now;
                    int day = dt.Month;
                    //int i = 0;
                    string EndDATE = string.Empty;
                    string StartDATE = string.Empty;
                    for (int i = 0; i < 12; i++)
                    {
                        if (i > 0)
                            dt = dt.AddDays(-7);
                        if (dt.DayOfWeek.ToString() == "Monday")
                        {
                            StartDATE = dt.ToString("yyyy-MM-dd 00:00:00.000");
                            EndDATE = dt.AddDays(5).ToString("yyyy-MM-dd 00:00:00.000");
                        }
                        else if (dt.DayOfWeek.ToString() == "Tuesday")
                        {

                            StartDATE = dt.AddDays(-1).ToString("yyyy-MM-dd 00:00:00.000");
                            EndDATE = dt.AddDays(4).ToString("yyyy-MM-dd 00:00:00.000");
                        }
                        else if (dt.DayOfWeek.ToString() == "Wednesday")
                        {

                            StartDATE = dt.AddDays(-2).ToString("yyyy-MM-dd 00:00:00.000");
                            EndDATE = dt.AddDays(3).ToString("yyyy-MM-dd 00:00:00.000");
                        }
                        else if (dt.DayOfWeek.ToString() == "Thursday")
                        {

                            StartDATE = dt.AddDays(-3).ToString("yyyy-MM-dd 00:00:00.000");
                            EndDATE = dt.AddDays(2).ToString("yyyy-MM-dd 00:00:00.000");
                        }
                        else if (dt.DayOfWeek.ToString() == "Friday")
                        {

                            StartDATE = dt.AddDays(-4).ToString("yyyy-MM-dd 00:00:00.000");
                            EndDATE = dt.AddDays(1).ToString("yyyy-MM-dd 00:00:00.000");
                        }

                        using (SqlCommand cmd = new SqlCommand("getFII_INDEX_weekly_monthly", Conn) { CommandType = CommandType.StoredProcedure })
                        {
                            cmd.Parameters.AddWithValue("@startdate", StartDATE);
                            cmd.Parameters.AddWithValue("@enddate", EndDATE);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    fiidii.DATE = StartDATE + " TO " + EndDATE;
                                    if (!reader.IsDBNull(0))
                                        fiidii.futidx = Math.Round(Convert.ToDecimal(reader.GetValue(0)));
                                    if (!reader.IsDBNull(1))
                                        fiidii.optidx = Math.Round(Convert.ToDecimal(reader.GetValue(1)));
                                }
                            }


                        }

                        using (SqlCommand cmd2 = new SqlCommand("getFII_Stock_weekly_monthly", Conn) { CommandType = CommandType.StoredProcedure })
                        {
                            cmd2.Parameters.AddWithValue("@startdate", StartDATE);
                            cmd2.Parameters.AddWithValue("@enddate", EndDATE);

                            using (var reader2 = cmd2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    if (!reader2.IsDBNull(0))
                                        fiidii.futstk = Math.Round(Convert.ToDecimal(reader2.GetValue(0)));
                                    if (!reader2.IsDBNull(1))
                                        fiidii.optstk = Math.Round(Convert.ToDecimal(reader2.GetValue(1)));

                                }
                            }

                            li.Add(new fii_dii_single { DATE = fiidii.DATE, futidx = fiidii.futidx, optidx = fiidii.optidx, futstk = fiidii.futstk, optstk = fiidii.optstk });
                            fiidii.futidx = 0.0M;
                            fiidii.optidx = 0.0M;
                            fiidii.futstk = 0.0M;
                            fiidii.optstk = 0.0M;
                        }
                    }
                }

                if (validity.ToUpper() == "MONTHLY")
                {
                    DateTime dt = DateTime.Now;
                    int day = dt.Month;
                    string EndDATE = string.Empty;
                    string StartDATE = string.Empty;
                    for (int i = 0; i < 12; i++)
                    {
                        if (i > 0)
                        {
                            dt = dt.AddDays(-30);
                            day = dt.Month;
                        }
                        if (day == 1)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 00:00:00.000");
                        }
                        else if (day == 2)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-28 00:00:00.000");
                        }
                        else if (day == 3)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 00:00:00.000");
                        }
                        else if (day == 4)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-30 00:00:00.000");
                        }
                        else if (day == 5)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 00:00:00.000");
                        }
                        else if (day == 6)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-30 00:00:00.000");
                        }
                        else if (day == 7)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 00:00:00.000");
                        }
                        else if (day == 8)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 00:00:00.000");
                        }
                        else if (day == 9)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-30 00:00:00.000");
                        }
                        else if (day == 10)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 00:00:00.000");
                        }
                        else if (day == 11)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-30 00:00:00.000");
                        }
                        else if (day == 12)
                        {
                            dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                            StartDATE = dt.ToString("yyyy-MM-01 00:00:00.000");
                            EndDATE = dt.ToString("yyyy-MM-31 00:00:00.000");
                        }

                        using (SqlCommand cmd1 = new SqlCommand("getFII_INDEX_weekly_monthly", Conn) { CommandType = CommandType.StoredProcedure })
                        {
                            cmd1.Parameters.AddWithValue("@startdate", StartDATE);
                            cmd1.Parameters.AddWithValue("@enddate", EndDATE);
                            using (var reader1 = cmd1.ExecuteReader())
                            {
                                while (reader1.Read())
                                {
                                    fiidii.DATE = StartDATE + " TO " + EndDATE;
                                    if (!reader1.IsDBNull(0))
                                        fiidii.futidx = Math.Round(Convert.ToDecimal(reader1.GetValue(0)));
                                    if (!reader1.IsDBNull(1))
                                        fiidii.optidx = Math.Round(Convert.ToDecimal(reader1.GetValue(1)));

                                }

                            }
                        }
                        using (SqlCommand cmd2 = new SqlCommand("getFII_Stock_weekly_monthly", Conn) { CommandType = CommandType.StoredProcedure })
                        {
                            cmd2.Parameters.AddWithValue("@startdate", StartDATE);
                            cmd2.Parameters.AddWithValue("@enddate", EndDATE);

                            using (var reader2 = cmd2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    if (!reader2.IsDBNull(0))
                                        fiidii.futstk = Math.Round(Convert.ToDecimal(reader2.GetValue(0)));
                                    if (!reader2.IsDBNull(1))
                                        fiidii.optstk = Math.Round(Convert.ToDecimal(reader2.GetValue(1)));

                                }

                            }

                            li.Add(new fii_dii_single { DATE = fiidii.DATE, futidx = fiidii.futidx, optidx = fiidii.optidx, futstk = fiidii.futstk, optstk = fiidii.optstk });
                            fiidii.futidx = 0.0M;
                            fiidii.optidx = 0.0M;
                            fiidii.futstk = 0.0M;
                            fiidii.optstk = 0.0M;

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
    }
}
