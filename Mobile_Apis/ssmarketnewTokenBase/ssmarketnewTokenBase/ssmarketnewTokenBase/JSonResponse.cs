using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssmarketnewTokenBase
{
    public class JSonResponse
    {
        public JObject JSon(FRatesResponse list)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new DictionaryAsArrayResolver();
            var json1 = JsonConvert.SerializeObject(list, settings);
            //json1 = "{\"result\":" + json1 + "}";
            var objects1 = JObject.Parse(json1);
            return objects1;
        }

        public JObject JSon1(FRatesResponse1 list)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new DictionaryAsArrayResolver();
            var json1 = JsonConvert.SerializeObject(list, settings);
            //json1 = "{\"result\":" + json1 + "}";
            var objects1 = JObject.Parse(json1);
            return objects1;
        }
    }
}