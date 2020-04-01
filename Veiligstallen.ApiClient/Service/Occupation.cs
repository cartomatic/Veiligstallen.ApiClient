using System;
using System.Threading.Tasks;
using VeiligStallen.ApiClient.DataModel;

namespace VeiligStallen.ApiClient
{
    public partial class Service
    {
        /// <summary>
        /// Reads occupation data for given city & location and returns raw response; credentials supplied as user name & pass
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="cityCode"></param>
        /// <param name="locationId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public static async Task<OccupationDataResponse> GetOccupationDataPassThroughAsync(string user, string pass,
            string cityCode, string locationId, DateTime? dateFrom, DateTime? dateTo)
        {
            return await GetOccupationDataPassThroughInternalAsync(GetAuthorizationHeaderValue(user, pass), cityCode,
                locationId, dateFrom, dateTo);
        }

        /// <summary>
        /// Reads occupation data for given city & location and returns raw response; credentials supplied as authorization token
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="cityCode"></param>
        /// <param name="locationId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public static async Task<OccupationDataResponse> GetOccupationDataPassThroughAsync(string authToken,
            string cityCode, string locationId, DateTime? dateFrom, DateTime? dateTo)
        {
            return await GetOccupationDataPassThroughInternalAsync(GetAuthorizationHeaderValue(authToken), cityCode,
                locationId, dateFrom, dateTo);
        }

        /// <summary>
        /// Reads occupation data for given city & location and returns raw response; credentials supplied as authorization header value
        /// </summary>
        /// <param name="authHdr"></param>
        /// <param name="cityCode"></param>
        /// <param name="locationId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        private static async Task<OccupationDataResponse> GetOccupationDataPassThroughInternalAsync(string authHdr, string cityCode, string locationId, DateTime? dateFrom, DateTime? dateTo)
        {
            var cfg = ServiceConfig.Read();

            try
            {
                var occupationData = await ApiCall<OccupationDataResponse>(
                    cfg.Endpoint,
                    cfg.Routes.LocationOccupation
                        .Replace("{city_code}", cityCode)
                        .Replace("{location_id}", locationId)
                        .Replace("{date_from}", (dateFrom ?? DateTime.Now.Date.AddDays(-8)).ToString("s"))
                        .Replace("{date_to}", (dateTo ?? DateTime.Now.Date.AddDays(-1)).ToString("s")),
                    authHdr
                );

                return occupationData;
            }
            catch
            {
                //ignore
            }

            return null;
        }

        //TODO - so far no need to do something with occupation data
    }
}
