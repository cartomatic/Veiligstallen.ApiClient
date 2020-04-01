using Newtonsoft.Json;
using System;

namespace VeiligStallen.ApiClient.DataModel
{
    /// <summary>
    /// VeiligStallen.nl occupation data response
    /// </summary>
    public class OccupationDataResponse : DataResponse
    {
        public OccupationLocation[] Locations { get; set; }
    }


    /// <summary>
    /// Transaction model normalized for Trajan APIs
    /// </summary>
    public class OccupationLocation
    {
        [JsonProperty("locationid")]
        public string LocationId { get; set; }

        public Defaults Defaults { get; set; }

        public OccupationSource[] Sources { get; set; }
    }

    public class OccupationSource
    {
        public int Count { get; set; }
        public string Name { get; set; }

        public OccupationRaw[] Data { get; set; }
    }

    public class OccupationRaw
    {
        public bool Open { get; set; }

        public DateTime Timestamp { get; set; }

        public int Occupation { get; set; }
    }

}
