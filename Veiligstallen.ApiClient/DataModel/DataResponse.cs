using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace VeiligStallen.ApiClient.DataModel
{
    /// <summary>
    /// VeiligStallen.nl data response base
    /// </summary>
    public class DataResponse
    {
        public int? Count { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        [JsonProperty("citycode")] public string CityCode { get; set; }

        [JsonProperty("locationid")] public string LocationId { get; set; }

        public Defaults Defaults { get; set; }
    }

    /// <summary>
    /// Generic defaults dictionary
    /// </summary>
    public class Defaults : Dictionary<string, object>
    {
        public T Get<T>(string key)
            where T : struct
        {
            if (ContainsKey(key))
            {
                try
                {
                    return (T)this[key];
                }
                catch
                {
                    //ignore
                }
            }
            return default;
        }
    }
}
