using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace VeiligStallen.ApiClient.DataModel
{
    public class City
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public Location[] Locations { get; set; }
    }

    /// <summary>
    /// City model as output by the VeiligStallen.nl service
    /// </summary>
    public class CityRaw
    {
        [JsonProperty("citycode")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("locations")]
        public LocationRaw[] Locations { get; set; }
    }

    public static class CityRawIncomingExtensions
    {
        public static City AsCity(this CityRaw rawCity)
        {
            return new City
            {
                Code = rawCity.Code,
                Name = rawCity.Name,
                Locations = (rawCity.Locations ?? new LocationRaw[0]).Select(x => x.AsLocation()).ToArray()
            };
        }

        public static IEnumerable<City> AsCities(this IEnumerable<CityRaw> rawCities)
        {
            return rawCities.Select(x => x.AsCity());
        }
    }
}
