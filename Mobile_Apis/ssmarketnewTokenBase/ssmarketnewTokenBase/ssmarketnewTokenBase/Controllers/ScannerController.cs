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
using System.Web;
using System.Web.Http;

namespace ssmarketnewTokenBase.Controllers
{
    [Authorize]
    public class ScannerController : ApiController
    {
        [HttpGet]
        [Route("api/Scanner/Open_High")]
        public HttpResponseMessage Open_High(string Exch)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {

                using (SqlCommand cmd = new SqlCommand("Openhigh", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exch);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {


                        while (reader.Read())
                        {

                            Scan sc = new Scan();
                            if (!reader.IsDBNull(0))
                                sc.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                sc.open = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                sc.high = Convert.ToDecimal(reader.GetValue(2));

                            if (!reader.IsDBNull(3))
                                sc.ltp = Convert.ToDecimal(reader.GetValue(3));

                            if (!reader.IsDBNull(4))
                                sc.percentchange = Convert.ToDecimal(reader.GetValue(4));


                            li.Add(new Scan { symbol = sc.symbol, open = sc.open, high = sc.high, ltp = sc.ltp, percentchange = sc.percentchange });


                        }

                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }


        [HttpGet]
        [Route("api/Scanner/Open_Low")]
        public HttpResponseMessage Open_Low(string Exch)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("Openlow", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exch);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            Scan sc = new Scan();
                            if (!reader.IsDBNull(0))
                                sc.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                sc.open = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                sc.high = Convert.ToDecimal(reader.GetValue(2));

                            if (!reader.IsDBNull(3))
                                sc.ltp = Convert.ToDecimal(reader.GetValue(3));

                            if (!reader.IsDBNull(4))
                                sc.percentchange = Convert.ToDecimal(reader.GetValue(4));

                            li.Add(new Scan { symbol = sc.symbol, open = sc.open, high = sc.high, ltp = sc.ltp, percentchange = sc.percentchange });

                        }
                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }


        [HttpGet]
        [Route("api/Scanner/Gap_up")]

        public HttpResponseMessage Gap_up(string Exch, string Percent)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("Gapup", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exch);
                    cmd.Parameters.AddWithValue("@Percent", Percent);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            Gap sc = new Gap();

                            if (!reader.IsDBNull(0))
                                sc.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                sc.Prev_Close = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                sc.open = Convert.ToDecimal(reader.GetValue(2));

                            if (!reader.IsDBNull(3))
                                sc.ltp = Convert.ToDecimal(reader.GetValue(3));

                            if (!reader.IsDBNull(4))
                                sc.percentchange = Convert.ToDecimal(reader.GetValue(4));

                            li.Add(new Gap { symbol = sc.symbol, Prev_Close = sc.Prev_Close, open = sc.open, ltp = sc.ltp, percentchange = sc.percentchange });

                        }
                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }

        [HttpGet]
        [Route("api/Scanner/Gap_down")]
        public HttpResponseMessage Gap_down(string Exch, string Percent)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("Gapdown", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exch);
                    cmd.Parameters.AddWithValue("@Percent", Percent);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            Gap sc = new Gap();
                            if (!reader.IsDBNull(0))
                                sc.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                sc.Prev_Close = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                sc.open = Convert.ToDecimal(reader.GetValue(2));

                            if (!reader.IsDBNull(3))
                                sc.ltp = Convert.ToDecimal(reader.GetValue(3));

                            if (!reader.IsDBNull(4))
                                sc.percentchange = Convert.ToDecimal(reader.GetValue(4));

                            li.Add(new Gap { symbol = sc.symbol, Prev_Close = sc.Prev_Close, open = sc.open, ltp = sc.ltp, percentchange = sc.percentchange });

                        }
                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }

        [HttpGet]
        [Route("api/Scanner/Volume_Price_Positve")]
        public HttpResponseMessage Volume_Price_Positve(string Exch)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("VolPricePositve", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exch);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            Volume sc = new Volume();
                            if (!reader.IsDBNull(0))
                                sc.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                sc.Prev_Volume = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                sc.volume = Convert.ToDecimal(reader.GetValue(2));

                            if (!reader.IsDBNull(3))
                                sc.Prev_Close = Convert.ToDecimal(reader.GetValue(3));

                            if (!reader.IsDBNull(4))
                                sc.ltp = Convert.ToDecimal(reader.GetValue(4));

                            if (!reader.IsDBNull(5))
                                sc.percentchange = Convert.ToDecimal(reader.GetValue(5));

                            li.Add(new Volume { symbol = sc.symbol, Prev_Volume = sc.Prev_Volume, volume = sc.volume, Prev_Close = sc.Prev_Close, ltp = sc.ltp, percentchange = sc.percentchange });

                        }
                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }

        [HttpGet]
        [Route("api/Scanner/Vol_Price_Negative")]
        public HttpResponseMessage Vol_Price_Negative(string Exch)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("VolPriceNegative", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exch);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            Volume vo = new Volume();

                            if (!reader.IsDBNull(0))
                                vo.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                vo.Prev_Volume = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                vo.volume = Convert.ToDecimal(reader.GetValue(2));

                            if (!reader.IsDBNull(3))
                                vo.Prev_Close = Convert.ToDecimal(reader.GetValue(3));

                            if (!reader.IsDBNull(4))
                                vo.ltp = Convert.ToDecimal(reader.GetValue(4));

                            if (!reader.IsDBNull(5))
                                vo.percentchange = Convert.ToDecimal(reader.GetValue(5));

                            li.Add(new Volume { symbol = vo.symbol, Prev_Volume = vo.Prev_Volume, volume = vo.volume, Prev_Close = vo.Prev_Close, ltp = vo.ltp, percentchange = vo.percentchange });

                        }
                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }


        [HttpGet]
        [Route("api/Scanner/Long_Build_Up")]
        public HttpResponseMessage Long_Build_Up(string Exch)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("LongBuildUp", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exch);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        int cnt = 0;
                        while (reader.Read())
                        {

                            Short sc = new Short();
                            if (!reader.IsDBNull(0))
                                sc.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                sc.Prev_OI = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                sc.OI = Convert.ToDecimal(reader.GetValue(2));

                            if (!reader.IsDBNull(3))
                                sc.percentchange = Convert.ToDecimal(reader.GetValue(3));

                            li.Add(new Short { symbol = sc.symbol, Prev_OI = sc.Prev_OI, OI = sc.OI, percentchange = sc.percentchange });

                        }


                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }

        [HttpGet]
        [Route("api/Scanner/Long_Unwinding")]
        public HttpResponseMessage Long_Unwinding(string Exch)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("LongUnwinding", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exch);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            Short sc = new Short();
                            if (!reader.IsDBNull(0))
                                sc.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                sc.Prev_OI = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                sc.OI = Convert.ToDecimal(reader.GetValue(2));

                            if (!reader.IsDBNull(3))
                                sc.percentchange = Convert.ToDecimal(reader.GetValue(3));


                            li.Add(new Short { symbol = sc.symbol, Prev_OI = sc.Prev_OI, OI = sc.OI, percentchange = sc.percentchange });

                        }


                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }



        [HttpGet]
        [Route("api/Scanner/Short_Build_Up")]
        public HttpResponseMessage Short_Build_Up(string Exch)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("ShortBuildup", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exch);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            Short sc = new Short();
                            if (!reader.IsDBNull(0))
                                sc.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                sc.Prev_OI = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                sc.OI = Convert.ToDecimal(reader.GetValue(2));

                            if (!reader.IsDBNull(3))
                                sc.percentchange = Convert.ToDecimal(reader.GetValue(3));

                            li.Add(new Short { symbol = sc.symbol, Prev_OI = sc.Prev_OI, OI = sc.OI, percentchange = sc.percentchange });

                        }


                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }

        [HttpGet]
        [Route("api/Scanner/Short_Covering")]
        public HttpResponseMessage Short_Covering(string Exch)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("ShortCovering", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exch);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            Short sc = new Short();
                            if (!reader.IsDBNull(0))
                                sc.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                sc.Prev_OI = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                sc.OI = Convert.ToDecimal(reader.GetValue(2));

                            if (!reader.IsDBNull(3))
                                sc.percentchange = Convert.ToDecimal(reader.GetValue(3));

                            li.Add(new Short { symbol = sc.symbol, Prev_OI = sc.Prev_OI, OI = sc.OI, percentchange = sc.percentchange });

                        }
                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }


        [HttpGet]
        [Route("api/Scanner/Open_High_Crossed")]
        public HttpResponseMessage Open_High_Crossed(string Exch)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("OpenHighCrossed", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@exch", Exch);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            cross sc = new cross();
                            if (!reader.IsDBNull(0))
                                sc.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                sc.highprice = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                sc.timestamp = reader.GetDateTime(2);


                            li.Add(new cross { symbol = sc.symbol, highprice = sc.highprice, timestamp = sc.timestamp });

                        }
                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);

        }

        [HttpGet]
        [Route("api/Scanner/Open_Low_Crossed")]
        public HttpResponseMessage Open_Low_Crossed(string Exch)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("OpenLowCrossed", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@exch", Exch);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            cross sc = new cross();
                            if (!reader.IsDBNull(0))
                                sc.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                sc.highprice = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                sc.timestamp = reader.GetDateTime(2);

                            li.Add(new cross { symbol = sc.symbol, highprice = sc.highprice, timestamp = sc.timestamp });

                        }
                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);

        }

        [HttpGet]
        [Route("api/Scanner/Prev_High_Crossed")]
        public HttpResponseMessage Prev_High_Crossed(string Exch)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("PrevHighCrossed", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exch);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            crossed sc = new crossed();
                            if (!reader.IsDBNull(0))
                                sc.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                sc.Prev_high = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                sc.high = Convert.ToDecimal(reader.GetValue(2));

                            if (!reader.IsDBNull(3))
                                sc.ltp = Convert.ToDecimal(reader.GetValue(3));

                            li.Add(new crossed { symbol = sc.symbol, Prev_high = sc.Prev_high, high = sc.high, ltp = sc.ltp });

                        }
                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);

        }

        [HttpGet]
        [Route("api/Scanner/Prev_Low_Crossed")]
        public HttpResponseMessage Prev_Low_Crossed(string Exch)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("PrevLowCrossed", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Exch", Exch);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            crossed sc = new crossed();
                            if (!reader.IsDBNull(0))
                                sc.symbol = reader.GetString(0);

                            if (!reader.IsDBNull(1))
                                sc.Prev_low = Convert.ToDecimal(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                sc.low = Convert.ToDecimal(reader.GetValue(2));

                            if (!reader.IsDBNull(3))
                                sc.ltp = Convert.ToDecimal(reader.GetValue(3));

                            li.Add(new crossed { symbol = sc.symbol, Prev_high = sc.Prev_high, high = sc.high, ltp = sc.ltp });

                        }
                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);

        }

        [HttpGet]
        [Route("api/Scanner/VolUP_100PERCENT11")]
        public HttpResponseMessage VolUP_100PERCENT11(string exch, string date)
        {
            SqlConnection conn = Utils.feedconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("VolUP100PERCENT1", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@exch", exch);
                    cmd.Parameters.AddWithValue("@date", date);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            VOLUP _up = new VOLUP();
                            if (!reader.IsDBNull(0))
                                _up.symbol = reader.GetString(0);
                            if (!reader.IsDBNull(1))
                                _up.pre_volume = Convert.ToDouble(reader.GetValue(1));
                            if (!reader.IsDBNull(2))
                                _up.curr_volume = Convert.ToDouble(reader.GetValue(2));
                            if (!reader.IsDBNull(3))
                                _up.LTP = Convert.ToDouble(reader.GetValue(3));

                            if (_up.pre_volume == 0)
                            {
                                _up.percent = Convert.ToDecimal(_up.curr_volume);
                            }
                            else
                            {
                                _up.percent = Convert.ToDecimal((_up.curr_volume / _up.pre_volume) * 100);
                            }

                            li.Add(new VOLUP { symbol = _up.symbol, pre_volume = _up.pre_volume, curr_volume = _up.curr_volume, LTP = _up.LTP });

                        }
                    }
                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);

        }

        [HttpGet]
        [Route("api/Scanner/Heatmap")]
        public HttpResponseMessage Heatmap(int exch)
        {
            SqlConnection conn = Utils.conn1;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                DateTime Fromdate = DateTime.Now;
                string datetime = Fromdate.ToString("yyyy-MM-dd 00:00:00");
                int d = (int)Fromdate.DayOfWeek;
                if (d == 0)
                    datetime = Fromdate.AddDays(-2).ToString("yyyy-MM-dd 00:00:00");
                else if (d == 1)
                    datetime = Fromdate.AddDays(-3).ToString("yyyy-MM-dd 00:00:00");
                else
                {
                    Fromdate = DateTime.Now.AddDays(-1);
                    datetime = Fromdate.ToString("yyyy-MM-dd 00:00:00");
                }
                //string datetime = date + " 00:00:00.000";
                // DateTime dt = Convert.ToDateTime(datetime);
                using (var cmd = new SqlCommand("HeatmapPercent", conn) { CommandType = CommandType.StoredProcedure })
                {
                    try
                    {
                        cmd.Parameters.Add("@Exch", SqlDbType.Int).Value = exch;
                        cmd.Parameters.Add("@Date", SqlDbType.NVarChar).Value = datetime;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                heatmap hm = new heatmap();
                                if (!reader.IsDBNull(0))
                                    hm.symbol = reader.GetString(0);
                                if (!reader.IsDBNull(1))
                                    hm.Prev_ClosePrice = Convert.ToDouble(reader.GetValue(1));
                                if (!reader.IsDBNull(2))
                                    hm.LTP = Convert.ToDouble(reader.GetValue(2));
                                if (!reader.IsDBNull(3))
                                    hm.Difference = Decimal.Round(Convert.ToDecimal(reader.GetValue(3)), 2);
                                if (!reader.IsDBNull(4))
                                    hm.Percentchange = Decimal.Round(Convert.ToDecimal(reader.GetValue(4)), 2);
                                //hm.Message = "Success";
                                li.Add(new heatmap { symbol = hm.symbol, Prev_ClosePrice = hm.Prev_ClosePrice, LTP = hm.LTP, Difference = hm.Difference, Percentchange = hm.Percentchange });

                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        //hm.Message = Ex.Message;
                        //li.Add(new heatmap { Message = hm.Message });
                    }

                    _response.data = li;
                    _Common.status = "Success";
                    _Common.result = li.Count + " Records Found";
                    _response.description = _Common;
                }
            }
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);

        }

        [HttpGet]
        [Route("api/Scanner/EMAData")]
        public HttpResponseMessage EMAData(int exch)
        {
            SqlConnection conn = Utils.ohlcconn;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            FRatesResponse _response = new FRatesResponse();
            CommonResponse _Common = new CommonResponse();
            //IList<dynamic> _fList = new List<dynamic>();
            List<dynamic> li = new List<dynamic>();
            _Common.status = "Failed";
            _Common.result = li.Count + " Records Found";
            _response.description = _Common;
            HttpContext httpContext = HttpContext.Current;
            if (httpContext.Request.Headers.AllKeys.Contains("Postman-Token") || httpContext.Request.Headers.AllKeys.Contains("postman-token"))
            {
                _Common.status = "Failed";
                _Common.result = li.Count().ToString() + " record found";
                _response.description = _Common;
            }
            else
            {
                SortedDictionary<string, EMADATA> _SymbolwiseEMA = new SortedDictionary<string, EMADATA>();

            int DAY = (int)DateTime.Now.DayOfWeek;
            DateTime fromdate = DateTime.Now;
            if (DAY == 1)
            {
                fromdate = DateTime.Now.AddDays(-3);
            }
            else if (DAY == 0)
            {
                fromdate = DateTime.Now.AddDays(-2);
            }
            else
                fromdate = DateTime.Now.AddDays(-1);
            DateTime todate = DateTime.Now;
            string _fromDate = fromdate.ToString("yyyy-MM-dd 15:30:00.000");
            string _toDate = todate.ToString("yyyy-MM-dd 15:30:00.000");
            using (var cmd = new SqlCommand("GetEMADATA", conn) { CommandType = CommandType.StoredProcedure })
            {
                try
                {
                    cmd.Parameters.Add("@Exch", SqlDbType.Int).Value = exch;
                    cmd.Parameters.Add("@fromDate", SqlDbType.NVarChar).Value = _fromDate;
                    cmd.Parameters.Add("@toDate", SqlDbType.NVarChar).Value = _toDate;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EMADATA _emavalue = new EMADATA();
                            if (!reader.IsDBNull(0))
                                _emavalue.Usersymbol = reader.GetString(0);
                            if (!reader.IsDBNull(198))
                                _emavalue.symbol = reader.GetString(198);

                            if (!reader.IsDBNull(1))
                                _emavalue.ema5 = Convert.ToDouble(reader.GetValue(1));

                            if (!reader.IsDBNull(2))
                                _emavalue.ema6 = Convert.ToDouble(reader.GetValue(2));

                            if (!reader.IsDBNull(3))
                                _emavalue.ema7 = Convert.ToDouble(reader.GetValue(3));

                            if (!reader.IsDBNull(4))
                                _emavalue.ema8 = Convert.ToDouble(reader.GetValue(4));

                            if (!reader.IsDBNull(5))
                                _emavalue.ema9 = Convert.ToDouble(reader.GetValue(5));

                            if (!reader.IsDBNull(6))
                                _emavalue.ema10 = Convert.ToDouble(reader.GetValue(6));

                            if (!reader.IsDBNull(7))
                                _emavalue.ema11 = Convert.ToDouble(reader.GetValue(7));

                            if (!reader.IsDBNull(8))
                                _emavalue.ema12 = Convert.ToDouble(reader.GetValue(8));

                            if (!reader.IsDBNull(9))
                                _emavalue.ema13 = Convert.ToDouble(reader.GetValue(9));

                            if (!reader.IsDBNull(10))
                                _emavalue.ema14 = Convert.ToDouble(reader.GetValue(10));

                            if (!reader.IsDBNull(11))
                                _emavalue.ema15 = Convert.ToDouble(reader.GetValue(11));

                            if (!reader.IsDBNull(12))
                                _emavalue.ema16 = Convert.ToDouble(reader.GetValue(12));

                            if (!reader.IsDBNull(13))
                                _emavalue.ema17 = Convert.ToDouble(reader.GetValue(13));

                            if (!reader.IsDBNull(14))
                                _emavalue.ema18 = Convert.ToDouble(reader.GetValue(14));

                            if (!reader.IsDBNull(15))
                                _emavalue.ema19 = Convert.ToDouble(reader.GetValue(15));

                            if (!reader.IsDBNull(16))
                                _emavalue.ema20 = Convert.ToDouble(reader.GetValue(16));

                            if (!reader.IsDBNull(17))
                                _emavalue.ema21 = Convert.ToDouble(reader.GetValue(17));

                            if (!reader.IsDBNull(18))
                                _emavalue.ema22 = Convert.ToDouble(reader.GetValue(18));

                            if (!reader.IsDBNull(19))
                                _emavalue.ema23 = Convert.ToDouble(reader.GetValue(19));

                            if (!reader.IsDBNull(20))
                                _emavalue.ema24 = Convert.ToDouble(reader.GetValue(20));

                            if (!reader.IsDBNull(21))
                                _emavalue.ema25 = Convert.ToDouble(reader.GetValue(21));

                            if (!reader.IsDBNull(22))
                                _emavalue.ema26 = Convert.ToDouble(reader.GetValue(22));

                            if (!reader.IsDBNull(23))
                                _emavalue.ema27 = Convert.ToDouble(reader.GetValue(23));

                            if (!reader.IsDBNull(24))
                                _emavalue.ema28 = Convert.ToDouble(reader.GetValue(24));

                            if (!reader.IsDBNull(25))
                                _emavalue.ema29 = Convert.ToDouble(reader.GetValue(25));

                            if (!reader.IsDBNull(26))
                                _emavalue.ema30 = Convert.ToDouble(reader.GetValue(26));

                            if (!reader.IsDBNull(27))
                                _emavalue.ema31 = Convert.ToDouble(reader.GetValue(27));

                            if (!reader.IsDBNull(28))
                                _emavalue.ema32 = Convert.ToDouble(reader.GetValue(28));

                            if (!reader.IsDBNull(29))
                                _emavalue.ema33 = Convert.ToDouble(reader.GetValue(29));

                            if (!reader.IsDBNull(30))
                                _emavalue.ema34 = Convert.ToDouble(reader.GetValue(30));

                            if (!reader.IsDBNull(31))
                                _emavalue.ema35 = Convert.ToDouble(reader.GetValue(31));

                            if (!reader.IsDBNull(32))
                                _emavalue.ema36 = Convert.ToDouble(reader.GetValue(32));

                            if (!reader.IsDBNull(33))
                                _emavalue.ema37 = Convert.ToDouble(reader.GetValue(33));

                            if (!reader.IsDBNull(34))
                                _emavalue.ema38 = Convert.ToDouble(reader.GetValue(34));

                            if (!reader.IsDBNull(35))
                                _emavalue.ema39 = Convert.ToDouble(reader.GetValue(35));

                            if (!reader.IsDBNull(36))
                                _emavalue.ema40 = Convert.ToDouble(reader.GetValue(36));

                            if (!reader.IsDBNull(37))
                                _emavalue.ema41 = Convert.ToDouble(reader.GetValue(37));

                            if (!reader.IsDBNull(38))
                                _emavalue.ema42 = Convert.ToDouble(reader.GetValue(38));

                            if (!reader.IsDBNull(39))
                                _emavalue.ema43 = Convert.ToDouble(reader.GetValue(39));

                            if (!reader.IsDBNull(40))
                                _emavalue.ema44 = Convert.ToDouble(reader.GetValue(40));

                            if (!reader.IsDBNull(41))
                                _emavalue.ema45 = Convert.ToDouble(reader.GetValue(41));

                            if (!reader.IsDBNull(42))
                                _emavalue.ema46 = Convert.ToDouble(reader.GetValue(42));

                            if (!reader.IsDBNull(43))
                                _emavalue.ema47 = Convert.ToDouble(reader.GetValue(43));

                            if (!reader.IsDBNull(44))
                                _emavalue.ema48 = Convert.ToDouble(reader.GetValue(44));

                            if (!reader.IsDBNull(45))
                                _emavalue.ema49 = Convert.ToDouble(reader.GetValue(45));

                            if (!reader.IsDBNull(46))
                                _emavalue.ema50 = Convert.ToDouble(reader.GetValue(46));

                            if (!reader.IsDBNull(47))
                                _emavalue.ema51 = Convert.ToDouble(reader.GetValue(47));

                            if (!reader.IsDBNull(48))
                                _emavalue.ema52 = Convert.ToDouble(reader.GetValue(48));

                            if (!reader.IsDBNull(49))
                                _emavalue.ema53 = Convert.ToDouble(reader.GetValue(49));

                            if (!reader.IsDBNull(50))
                                _emavalue.ema54 = Convert.ToDouble(reader.GetValue(50));

                            if (!reader.IsDBNull(51))
                                _emavalue.ema55 = Convert.ToDouble(reader.GetValue(51));

                            if (!reader.IsDBNull(52))
                                _emavalue.ema56 = Convert.ToDouble(reader.GetValue(52));

                            if (!reader.IsDBNull(53))
                                _emavalue.ema57 = Convert.ToDouble(reader.GetValue(53));

                            if (!reader.IsDBNull(54))
                                _emavalue.ema58 = Convert.ToDouble(reader.GetValue(54));

                            if (!reader.IsDBNull(55))
                                _emavalue.ema59 = Convert.ToDouble(reader.GetValue(55));

                            if (!reader.IsDBNull(56))
                                _emavalue.ema60 = Convert.ToDouble(reader.GetValue(56));

                            if (!reader.IsDBNull(57))
                                _emavalue.ema61 = Convert.ToDouble(reader.GetValue(57));

                            if (!reader.IsDBNull(58))
                                _emavalue.ema62 = Convert.ToDouble(reader.GetValue(58));

                            if (!reader.IsDBNull(59))
                                _emavalue.ema63 = Convert.ToDouble(reader.GetValue(59));

                            if (!reader.IsDBNull(60))
                                _emavalue.ema64 = Convert.ToDouble(reader.GetValue(60));

                            if (!reader.IsDBNull(61))
                                _emavalue.ema65 = Convert.ToDouble(reader.GetValue(61));

                            if (!reader.IsDBNull(62))
                                _emavalue.ema66 = Convert.ToDouble(reader.GetValue(62));

                            if (!reader.IsDBNull(63))
                                _emavalue.ema67 = Convert.ToDouble(reader.GetValue(63));

                            if (!reader.IsDBNull(64))
                                _emavalue.ema68 = Convert.ToDouble(reader.GetValue(64));

                            if (!reader.IsDBNull(65))
                                _emavalue.ema69 = Convert.ToDouble(reader.GetValue(65));

                            if (!reader.IsDBNull(66))
                                _emavalue.ema70 = Convert.ToDouble(reader.GetValue(66));

                            if (!reader.IsDBNull(67))
                                _emavalue.ema71 = Convert.ToDouble(reader.GetValue(67));

                            if (!reader.IsDBNull(68))
                                _emavalue.ema72 = Convert.ToDouble(reader.GetValue(68));

                            if (!reader.IsDBNull(69))
                                _emavalue.ema73 = Convert.ToDouble(reader.GetValue(69));

                            if (!reader.IsDBNull(70))
                                _emavalue.ema74 = Convert.ToDouble(reader.GetValue(70));

                            if (!reader.IsDBNull(71))
                                _emavalue.ema75 = Convert.ToDouble(reader.GetValue(71));

                            if (!reader.IsDBNull(72))
                                _emavalue.ema76 = Convert.ToDouble(reader.GetValue(72));

                            if (!reader.IsDBNull(73))
                                _emavalue.ema77 = Convert.ToDouble(reader.GetValue(73));

                            if (!reader.IsDBNull(74))
                                _emavalue.ema78 = Convert.ToDouble(reader.GetValue(74));

                            if (!reader.IsDBNull(75))
                                _emavalue.ema79 = Convert.ToDouble(reader.GetValue(75));

                            if (!reader.IsDBNull(76))
                                _emavalue.ema80 = Convert.ToDouble(reader.GetValue(76));

                            if (!reader.IsDBNull(77))
                                _emavalue.ema81 = Convert.ToDouble(reader.GetValue(77));

                            if (!reader.IsDBNull(78))
                                _emavalue.ema82 = Convert.ToDouble(reader.GetValue(78));

                            if (!reader.IsDBNull(79))
                                _emavalue.ema83 = Convert.ToDouble(reader.GetValue(79));

                            if (!reader.IsDBNull(80))
                                _emavalue.ema84 = Convert.ToDouble(reader.GetValue(80));

                            if (!reader.IsDBNull(81))
                                _emavalue.ema85 = Convert.ToDouble(reader.GetValue(81));

                            if (!reader.IsDBNull(82))
                                _emavalue.ema86 = Convert.ToDouble(reader.GetValue(82));

                            if (!reader.IsDBNull(83))
                                _emavalue.ema87 = Convert.ToDouble(reader.GetValue(83));

                            if (!reader.IsDBNull(84))
                                _emavalue.ema88 = Convert.ToDouble(reader.GetValue(84));

                            if (!reader.IsDBNull(85))
                                _emavalue.ema89 = Convert.ToDouble(reader.GetValue(85));

                            if (!reader.IsDBNull(86))
                                _emavalue.ema90 = Convert.ToDouble(reader.GetValue(86));

                            if (!reader.IsDBNull(87))
                                _emavalue.ema91 = Convert.ToDouble(reader.GetValue(87));

                            if (!reader.IsDBNull(88))
                                _emavalue.ema92 = Convert.ToDouble(reader.GetValue(88));

                            if (!reader.IsDBNull(89))
                                _emavalue.ema93 = Convert.ToDouble(reader.GetValue(89));

                            if (!reader.IsDBNull(90))
                                _emavalue.ema94 = Convert.ToDouble(reader.GetValue(90));

                            if (!reader.IsDBNull(91))
                                _emavalue.ema95 = Convert.ToDouble(reader.GetValue(91));

                            if (!reader.IsDBNull(92))
                                _emavalue.ema96 = Convert.ToDouble(reader.GetValue(92));

                            if (!reader.IsDBNull(93))
                                _emavalue.ema97 = Convert.ToDouble(reader.GetValue(93));

                            if (!reader.IsDBNull(94))
                                _emavalue.ema98 = Convert.ToDouble(reader.GetValue(94));

                            if (!reader.IsDBNull(95))
                                _emavalue.ema99 = Convert.ToDouble(reader.GetValue(95));

                            if (!reader.IsDBNull(96))
                                _emavalue.ema100 = Convert.ToDouble(reader.GetValue(96));

                            if (!reader.IsDBNull(97))
                                _emavalue.ema101 = Convert.ToDouble(reader.GetValue(97));

                            if (!reader.IsDBNull(98))
                                _emavalue.ema102 = Convert.ToDouble(reader.GetValue(98));

                            if (!reader.IsDBNull(99))
                                _emavalue.ema103 = Convert.ToDouble(reader.GetValue(99));

                            if (!reader.IsDBNull(100))
                                _emavalue.ema104 = Convert.ToDouble(reader.GetValue(100));

                            if (!reader.IsDBNull(101))
                                _emavalue.ema105 = Convert.ToDouble(reader.GetValue(101));

                            if (!reader.IsDBNull(102))
                                _emavalue.ema106 = Convert.ToDouble(reader.GetValue(102));

                            if (!reader.IsDBNull(103))
                                _emavalue.ema107 = Convert.ToDouble(reader.GetValue(103));

                            if (!reader.IsDBNull(104))
                                _emavalue.ema108 = Convert.ToDouble(reader.GetValue(104));

                            if (!reader.IsDBNull(105))
                                _emavalue.ema109 = Convert.ToDouble(reader.GetValue(105));

                            if (!reader.IsDBNull(106))
                                _emavalue.ema110 = Convert.ToDouble(reader.GetValue(106));

                            if (!reader.IsDBNull(107))
                                _emavalue.ema111 = Convert.ToDouble(reader.GetValue(107));

                            if (!reader.IsDBNull(108))
                                _emavalue.ema112 = Convert.ToDouble(reader.GetValue(108));

                            if (!reader.IsDBNull(109))
                                _emavalue.ema113 = Convert.ToDouble(reader.GetValue(109));

                            if (!reader.IsDBNull(110))
                                _emavalue.ema114 = Convert.ToDouble(reader.GetValue(110));

                            if (!reader.IsDBNull(111))
                                _emavalue.ema115 = Convert.ToDouble(reader.GetValue(111));

                            if (!reader.IsDBNull(112))
                                _emavalue.ema116 = Convert.ToDouble(reader.GetValue(112));

                            if (!reader.IsDBNull(113))
                                _emavalue.ema117 = Convert.ToDouble(reader.GetValue(113));

                            if (!reader.IsDBNull(114))
                                _emavalue.ema118 = Convert.ToDouble(reader.GetValue(114));

                            if (!reader.IsDBNull(115))
                                _emavalue.ema119 = Convert.ToDouble(reader.GetValue(115));

                            if (!reader.IsDBNull(116))
                                _emavalue.ema120 = Convert.ToDouble(reader.GetValue(116));

                            if (!reader.IsDBNull(117))
                                _emavalue.ema121 = Convert.ToDouble(reader.GetValue(117));

                            if (!reader.IsDBNull(118))
                                _emavalue.ema122 = Convert.ToDouble(reader.GetValue(118));

                            if (!reader.IsDBNull(119))
                                _emavalue.ema123 = Convert.ToDouble(reader.GetValue(119));

                            if (!reader.IsDBNull(120))
                                _emavalue.ema124 = Convert.ToDouble(reader.GetValue(120));

                            if (!reader.IsDBNull(121))
                                _emavalue.ema125 = Convert.ToDouble(reader.GetValue(121));

                            if (!reader.IsDBNull(122))
                                _emavalue.ema126 = Convert.ToDouble(reader.GetValue(122));

                            if (!reader.IsDBNull(123))
                                _emavalue.ema127 = Convert.ToDouble(reader.GetValue(123));

                            if (!reader.IsDBNull(124))
                                _emavalue.ema128 = Convert.ToDouble(reader.GetValue(124));

                            if (!reader.IsDBNull(125))
                                _emavalue.ema129 = Convert.ToDouble(reader.GetValue(125));

                            if (!reader.IsDBNull(126))
                                _emavalue.ema130 = Convert.ToDouble(reader.GetValue(126));

                            if (!reader.IsDBNull(127))
                                _emavalue.ema131 = Convert.ToDouble(reader.GetValue(127));

                            if (!reader.IsDBNull(128))
                                _emavalue.ema132 = Convert.ToDouble(reader.GetValue(128));

                            if (!reader.IsDBNull(129))
                                _emavalue.ema133 = Convert.ToDouble(reader.GetValue(129));

                            if (!reader.IsDBNull(130))
                                _emavalue.ema134 = Convert.ToDouble(reader.GetValue(130));

                            if (!reader.IsDBNull(131))
                                _emavalue.ema135 = Convert.ToDouble(reader.GetValue(131));

                            if (!reader.IsDBNull(132))
                                _emavalue.ema136 = Convert.ToDouble(reader.GetValue(132));

                            if (!reader.IsDBNull(133))
                                _emavalue.ema137 = Convert.ToDouble(reader.GetValue(133));

                            if (!reader.IsDBNull(134))
                                _emavalue.ema138 = Convert.ToDouble(reader.GetValue(134));

                            if (!reader.IsDBNull(135))
                                _emavalue.ema139 = Convert.ToDouble(reader.GetValue(135));

                            if (!reader.IsDBNull(136))
                                _emavalue.ema140 = Convert.ToDouble(reader.GetValue(136));

                            if (!reader.IsDBNull(137))
                                _emavalue.ema141 = Convert.ToDouble(reader.GetValue(137));

                            if (!reader.IsDBNull(138))
                                _emavalue.ema142 = Convert.ToDouble(reader.GetValue(138));

                            if (!reader.IsDBNull(139))
                                _emavalue.ema143 = Convert.ToDouble(reader.GetValue(139));

                            if (!reader.IsDBNull(140))
                                _emavalue.ema144 = Convert.ToDouble(reader.GetValue(140));

                            if (!reader.IsDBNull(141))
                                _emavalue.ema145 = Convert.ToDouble(reader.GetValue(141));

                            if (!reader.IsDBNull(142))
                                _emavalue.ema146 = Convert.ToDouble(reader.GetValue(142));

                            if (!reader.IsDBNull(143))
                                _emavalue.ema147 = Convert.ToDouble(reader.GetValue(143));

                            if (!reader.IsDBNull(144))
                                _emavalue.ema148 = Convert.ToDouble(reader.GetValue(144));

                            if (!reader.IsDBNull(145))
                                _emavalue.ema149 = Convert.ToDouble(reader.GetValue(145));

                            if (!reader.IsDBNull(146))
                                _emavalue.ema150 = Convert.ToDouble(reader.GetValue(146));

                            if (!reader.IsDBNull(147))
                                _emavalue.ema151 = Convert.ToDouble(reader.GetValue(147));

                            if (!reader.IsDBNull(148))
                                _emavalue.ema152 = Convert.ToDouble(reader.GetValue(148));

                            if (!reader.IsDBNull(149))
                                _emavalue.ema153 = Convert.ToDouble(reader.GetValue(149));

                            if (!reader.IsDBNull(150))
                                _emavalue.ema154 = Convert.ToDouble(reader.GetValue(150));

                            if (!reader.IsDBNull(151))
                                _emavalue.ema155 = Convert.ToDouble(reader.GetValue(151));

                            if (!reader.IsDBNull(152))
                                _emavalue.ema156 = Convert.ToDouble(reader.GetValue(152));

                            if (!reader.IsDBNull(153))
                                _emavalue.ema157 = Convert.ToDouble(reader.GetValue(153));

                            if (!reader.IsDBNull(154))
                                _emavalue.ema158 = Convert.ToDouble(reader.GetValue(154));

                            if (!reader.IsDBNull(155))
                                _emavalue.ema159 = Convert.ToDouble(reader.GetValue(155));

                            if (!reader.IsDBNull(156))
                                _emavalue.ema160 = Convert.ToDouble(reader.GetValue(156));

                            if (!reader.IsDBNull(157))
                                _emavalue.ema161 = Convert.ToDouble(reader.GetValue(157));

                            if (!reader.IsDBNull(158))
                                _emavalue.ema162 = Convert.ToDouble(reader.GetValue(158));

                            if (!reader.IsDBNull(159))
                                _emavalue.ema163 = Convert.ToDouble(reader.GetValue(159));

                            if (!reader.IsDBNull(160))
                                _emavalue.ema164 = Convert.ToDouble(reader.GetValue(160));

                            if (!reader.IsDBNull(161))
                                _emavalue.ema165 = Convert.ToDouble(reader.GetValue(161));

                            if (!reader.IsDBNull(162))
                                _emavalue.ema166 = Convert.ToDouble(reader.GetValue(162));

                            if (!reader.IsDBNull(163))
                                _emavalue.ema167 = Convert.ToDouble(reader.GetValue(163));

                            if (!reader.IsDBNull(164))
                                _emavalue.ema168 = Convert.ToDouble(reader.GetValue(164));

                            if (!reader.IsDBNull(165))
                                _emavalue.ema169 = Convert.ToDouble(reader.GetValue(165));

                            if (!reader.IsDBNull(166))
                                _emavalue.ema170 = Convert.ToDouble(reader.GetValue(166));

                            if (!reader.IsDBNull(167))
                                _emavalue.ema171 = Convert.ToDouble(reader.GetValue(167));

                            if (!reader.IsDBNull(168))
                                _emavalue.ema172 = Convert.ToDouble(reader.GetValue(168));

                            if (!reader.IsDBNull(169))
                                _emavalue.ema173 = Convert.ToDouble(reader.GetValue(169));

                            if (!reader.IsDBNull(170))
                                _emavalue.ema174 = Convert.ToDouble(reader.GetValue(170));

                            if (!reader.IsDBNull(171))
                                _emavalue.ema175 = Convert.ToDouble(reader.GetValue(171));

                            if (!reader.IsDBNull(172))
                                _emavalue.ema176 = Convert.ToDouble(reader.GetValue(172));

                            if (!reader.IsDBNull(173))
                                _emavalue.ema177 = Convert.ToDouble(reader.GetValue(173));

                            if (!reader.IsDBNull(174))
                                _emavalue.ema178 = Convert.ToDouble(reader.GetValue(174));

                            if (!reader.IsDBNull(175))
                                _emavalue.ema179 = Convert.ToDouble(reader.GetValue(175));

                            if (!reader.IsDBNull(176))
                                _emavalue.ema180 = Convert.ToDouble(reader.GetValue(176));

                            if (!reader.IsDBNull(177))
                                _emavalue.ema181 = Convert.ToDouble(reader.GetValue(177));

                            if (!reader.IsDBNull(178))
                                _emavalue.ema182 = Convert.ToDouble(reader.GetValue(178));

                            if (!reader.IsDBNull(179))
                                _emavalue.ema183 = Convert.ToDouble(reader.GetValue(179));

                            if (!reader.IsDBNull(180))
                                _emavalue.ema184 = Convert.ToDouble(reader.GetValue(180));

                            if (!reader.IsDBNull(181))
                                _emavalue.ema185 = Convert.ToDouble(reader.GetValue(181));

                            if (!reader.IsDBNull(182))
                                _emavalue.ema186 = Convert.ToDouble(reader.GetValue(182));

                            if (!reader.IsDBNull(183))
                                _emavalue.ema187 = Convert.ToDouble(reader.GetValue(183));

                            if (!reader.IsDBNull(184))
                                _emavalue.ema188 = Convert.ToDouble(reader.GetValue(184));

                            if (!reader.IsDBNull(185))
                                _emavalue.ema189 = Convert.ToDouble(reader.GetValue(185));

                            if (!reader.IsDBNull(186))
                                _emavalue.ema190 = Convert.ToDouble(reader.GetValue(186));

                            if (!reader.IsDBNull(187))
                                _emavalue.ema191 = Convert.ToDouble(reader.GetValue(187));

                            if (!reader.IsDBNull(188))
                                _emavalue.ema192 = Convert.ToDouble(reader.GetValue(188));

                            if (!reader.IsDBNull(189))
                                _emavalue.ema193 = Convert.ToDouble(reader.GetValue(189));

                            if (!reader.IsDBNull(190))
                                _emavalue.ema194 = Convert.ToDouble(reader.GetValue(190));

                            if (!reader.IsDBNull(191))
                                _emavalue.ema195 = Convert.ToDouble(reader.GetValue(191));

                            if (!reader.IsDBNull(192))
                                _emavalue.ema196 = Convert.ToDouble(reader.GetValue(192));

                            if (!reader.IsDBNull(193))
                                _emavalue.ema197 = Convert.ToDouble(reader.GetValue(193));

                            if (!reader.IsDBNull(194))
                                _emavalue.ema198 = Convert.ToDouble(reader.GetValue(194));

                            if (!reader.IsDBNull(195))
                                _emavalue.ema199 = Convert.ToDouble(reader.GetValue(195));

                            if (!reader.IsDBNull(196))
                                _emavalue.ema200 = Convert.ToDouble(reader.GetValue(196));

                            if (!reader.IsDBNull(197))
                                _emavalue.emadate = Convert.ToDateTime(reader.GetValue(197));
                            if (!_SymbolwiseEMA.ContainsKey(_emavalue.symbol))
                            {
                                _SymbolwiseEMA.Add(_emavalue.symbol, _emavalue);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new DictionaryAsArrayResolver();
            var json1 = JsonConvert.SerializeObject(_SymbolwiseEMA, settings);
            json1 = "{\"data\":" + json1 + "}";
            var objects1 = JObject.Parse(json1);
            return Request.CreateResponse(HttpStatusCode.OK, objects1);

            }
            _Common.status = "Failed";
            _Common.result = li.Count().ToString() + " record found";
            _response.description = _Common;
            JSonResponse js = new JSonResponse();
            var object1 = js.JSon(_response);
            return Request.CreateResponse(HttpStatusCode.OK, object1);
        }
        //_response.data = li;
        //    _Common.status = "Success";
        //    _Common.result = li.Count + " Records Found";
        //    _response.description = _Common;


        //JSonResponse js = new JSonResponse();
        //var object1 = js.JSon(_response);
        //return Request.CreateResponse(HttpStatusCode.OK, object1);

    }

}


