using Newtonsoft.Json;

namespace VeiligStallen.ApiClient.DataModel
{
    /// <summary>
    /// auth model as output by the VeiligStallen.nl service
    /// </summary>
    public class AuthRaw
    {
        public string Company { get; set; }

        public string Name { get; set; }

        [JsonProperty("loginname")]
        public string LoginName { get; set; }

        public CityRaw[] Cities { get; set; }
    }

}
