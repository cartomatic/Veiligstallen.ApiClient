namespace VeiligStallen.ApiClient
{
    public partial class Service
    {
        /// <summary>
        /// Gets auth hdr value
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        private static string GetAuthorizationHeaderValue(string user, string pass)
        {
            var cfg = ServiceConfig.Read();

            switch (cfg?.AuthorizationScheme?.ToLower())
            {
                case "basic":
                    return GetAuthorizationHeaderValue(
                        System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{user}:{pass}"))
                    );
            }

            return null;
        }

        /// <summary>
        /// Gets auth hdr value
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        private static string GetAuthorizationHeaderValue(string authToken)
        {
            var cfg = ServiceConfig.Read();
            return $"{cfg?.AuthorizationScheme} {authToken}";
        }
    }
}
