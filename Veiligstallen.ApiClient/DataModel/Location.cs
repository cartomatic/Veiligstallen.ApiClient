using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace VeiligStallen.ApiClient.DataModel
{
    public class Location
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Id { get; set; }

        public virtual double? Lo { get; set; }

        public virtual double? La { get; set; }
    }

    /// <summary>
    /// Location model as output by the VeiligStallen.nl service
    /// </summary>
    public class LocationRaw
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("locationtype")]
        public string LocationType { get; set; }

        [JsonProperty("locationid")]
        public string LocationId { get; set; }

        [JsonProperty("long")]
        public double? Lo { get; set; }

        [JsonProperty("lat")]
        public double? La { get; set; }
    }

    public static class LocationRawIncomingExtensions
    {
        public static Location AsLocation(this LocationRaw rawLocation)
        {
            return new Location
            {
                Id = rawLocation.LocationId,
                Name = rawLocation.Name,
                Type = rawLocation.Type ?? rawLocation.LocationType,
                Lo = rawLocation.Lo,
                La = rawLocation.La
            };
        }

        public static IEnumerable<Location> AsLocations(this IEnumerable<LocationRaw> rawLocations)
        {
            return rawLocations.Select(x => x.AsLocation());
        }
    }


}
