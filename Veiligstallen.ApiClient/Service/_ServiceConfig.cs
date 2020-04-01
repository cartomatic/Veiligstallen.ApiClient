using Microsoft.Extensions.Configuration;

namespace VeiligStallen.ApiClient
{
    public class ServiceConfig
    {
        public string AuthorizationScheme { get; set; }

        public string Endpoint { get; set; }

        public Routes Routes { get; set; }

        private static ServiceConfig _cfg;

        public static ServiceConfig Read()
        {
            if (_cfg != null)
                return _cfg;

            var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

            _cfg = cfg.GetSection("VeiligStallen").Get<ServiceConfig>();

            return _cfg;
        }
    }

    public class Routes
    {
        public string Auth { get; set; }

        public string CityLocations { get; set; }

        public string LocationTransactions { get; set; }

        public string LocationOccupation { get; set; }
    }
}
