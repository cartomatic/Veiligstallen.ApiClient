using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace VeiligStallen.ApiClient
{
    public partial class Service
    {
        /// <summary>
        /// Generic external api caller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="auth">Authorization header value</param>
        /// <returns></returns>
        private static async Task<T> ApiCall<T>(string url, string route, string auth)
            where T : class
        {
            var output = default(T);

            //Note:
            //veiligstallen api does not seem to be happy with some restsharp client calls (specifically with the auth call)
            //therefore just using a std client instead
            //try
            //{
            //    var apiCall = await Cartomatic.Utils.RestApi.RestApiCall<T>(
            //        url,
            //        route,
            //        Method.GET,
            //        authToken: auth
            //    );

            //    if (apiCall.Response.IsSuccessful)
            //        output = apiCall.Output;
            //}
            //catch
            //{
            //    //ignore
            //}



            var request = System.Net.WebRequest.Create($"{url}{(url.EndsWith("/") ? "" : "/")}{route}");
            request.Method = "GET";
            request.Headers.Add("Authorization", auth);

            try
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        var respStr = response.GetResponseStream();
                        if (respStr != null)
                        {
                            using (var sr = new StreamReader(respStr))
                            {
                                output = JsonConvert.DeserializeObject<T>(await sr.ReadToEndAsync());
                            }
                        }

                    }
                }
            }
            catch
            {
                //ignore
            }

            return output;
        }


    }
}
