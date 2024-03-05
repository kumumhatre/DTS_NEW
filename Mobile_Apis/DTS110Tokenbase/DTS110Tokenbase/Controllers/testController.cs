using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DTS110Tokenbase.Controllers
{
    public class testController : ApiController
    {
        public HttpResponseMessage getSymbols(string exch)
        {
            //HttpContext httpContext = HttpContext.Current;
            //NameValueCollection headerList = httpContext.Request.Headers;
            //var authorizationField = headerList.Get("postman-token");
            test1 response = new test1();
            //CommonResponse common = new CommonResponse();
             List<test1> li = new List<test1>();
            
            SqlConnection feedConn = new SqlConnection("Data Source = 164.68.123.204\\M13504\\MSQLSERVER17,49170; User ID = sa; Initial Catalog = DTS1; Password =Mickey#496; Integrated Security = false; MultipleActiveResultSets = True; Connect Timeout = 500000000");
            if (feedConn.State != ConnectionState.Open)
            {
                feedConn.Open();
            }
            using (SqlCommand cmd = new SqlCommand("select * from test1", feedConn))
            {
                
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        test1 getsym = new test1();
                        if (!reader.IsDBNull(0))
                            getsym.id = reader.GetInt32(0);
                        if (!reader.IsDBNull(1))
                            getsym.name = reader.GetString(1);
                        getsym.data = getdata(getsym.id, feedConn);
                        li.Add(new test1 { id = getsym.id, name = getsym.name,data=getsym.data });
                    }
                }
            }
            //response.data = li;
            //common.status = "Success";
            //common.result = li.Count().ToString() + " record found";
            //response.description = common;
            // }                      
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new DictionaryAsArrayResolver();
            var json1 = JsonConvert.SerializeObject(li, settings);
            json1 = "{\"result\":" + json1 + "}";
            var objects1 = JObject.Parse(json1);
            return Request.CreateResponse(HttpStatusCode.OK, objects1);
        }
        [HttpPost]
        public HttpResponseMessage Postsymbol([FromBody]test1 exch)
        {
            //HttpContext httpContext = HttpContext.Current;
            //NameValueCollection headerList = httpContext.Request.Headers;
            //var authorizationField = headerList.Get("postman-token");
           // test1 response = new test1();
            //CommonResponse common = new CommonResponse();
           // List<test1> li = new List<test1>();

            SqlConnection feedConn = new SqlConnection("Data Source = 164.68.123.204\\M13504\\MSQLSERVER17,49170; User ID = sa; Initial Catalog = DTS1; Password =Mickey#496; Integrated Security = false; MultipleActiveResultSets = True; Connect Timeout = 500000000");
            if (feedConn.State != ConnectionState.Open)
            {
                feedConn.Open();
            }
            int i = 0;
            string msg = string.Empty;
            using (SqlCommand cmd = new SqlCommand("insert into test1 values("+exch.id+",'"+exch.name+"')", feedConn))
            {
                try
                {
                    Utils  _u = new Utils();
                     i = cmd.ExecuteNonQuery();
                    _u.data(exch.data,feedConn);
                }                   
                catch(Exception ex)
                {
                    msg = ex.Message;
                }
                if (i > 0)
                {
                    msg = "successfull";
                }
                else
                {
                    msg="failed  " +msg;
                }
                    
            }
            //response.data = li;
            //common.status = "Success";
            //common.result = li.Count().ToString() + " record found";
            //response.description = common;
            // }                      
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new DictionaryAsArrayResolver();
            var json1 = JsonConvert.SerializeObject(msg, settings);
            json1 = "{\"result\":" + json1 + "}";
            var objects1 = JObject.Parse(json1);
            return Request.CreateResponse(HttpStatusCode.OK, objects1);
        }
        public test2 getdata(int i, SqlConnection feedConn)
        {
            test2 getsym = new test2();
            using (SqlCommand cmd = new SqlCommand("select * from test2 where id=" + i, feedConn))
            {

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        if (!reader.IsDBNull(0))
                            getsym.id = reader.GetInt32(0);
                        if (!reader.IsDBNull(1))
                            getsym.Surname = reader.GetString(1);


                    }
                }
            }
            return getsym;
        }
        

    }
    
    public class test1
    {
        public int id { get; set; }
        public string name { get; set; }
        public test2 data { get; set; }
    }
    public class test2
    {
        public int id { get; set; }
        public string Surname { get; set; }
    }


}
