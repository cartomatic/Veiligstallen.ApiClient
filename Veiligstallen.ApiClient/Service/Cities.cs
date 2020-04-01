using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VeiligStallen.ApiClient.DataModel;

namespace VeiligStallen.ApiClient
{
    public partial class Service
    {
        /// <summary>
        /// Gets cities; credentials supplied as user name & pass
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<City>> GetCitiesAsync(string user, string pass)
        {
            return await GetCitiesInternalAsync(GetAuthorizationHeaderValue(user, pass));
        }

        /// <summary>
        /// Gets cities; credentials supplied as authorization token
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<City>> GetCitiesAsync(string authToken)
        {
            return await GetCitiesInternalAsync(GetAuthorizationHeaderValue(authToken));
        }


        /// <summary>
        /// Gets cities; credentials supplied as authorization header value
        /// </summary>
        /// <param name="authHdr"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<City>> GetCitiesInternalAsync(string authHdr)
        {
            var cfg = ServiceConfig.Read();

            //Note: cities are obtained by calling the auth endpoint

            var authCall = await ApiCall<AuthRaw>(
                cfg.Endpoint,
                cfg.Routes.Auth,
                authHdr
            );

            if (authCall == null)
                return null;

            //at this stage should have some data - in particular cities
            var cities = authCall.Cities.AsCities().ToArray();


            //need to obtain location for each city
            foreach (var city in cities)
            {
                var rawLocations = await ApiCall<LocationRaw[]>(
                    cfg.Endpoint,
                    cfg.Routes.CityLocations.Replace("{city_code}", city.Code),
                    authHdr
                );

                if (rawLocations == null)
                    continue;

                var locations = rawLocations.AsLocations();

                foreach (var cityLoc in city.Locations)
                {
                    var loc = locations.FirstOrDefault(x => x.Id == cityLoc.Id);
                    cityLoc.Lo = loc?.Lo;
                    cityLoc.La = loc?.La;
                }
            }

            return cities;
        }
    }
}
