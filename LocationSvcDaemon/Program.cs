using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocationSvcDaemon
{
    class Program
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:appKey"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:aadInstance"];
        private static string tenant = ConfigurationManager.AppSettings["ida:tenant"];
        private static string serviceResourceId = ConfigurationManager.AppSettings["ida:serviceResourceID"];

        
        private static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        private static AuthenticationContext authContext = new AuthenticationContext(authority);
        private static ClientCredential clientCredential = new ClientCredential(clientId, appKey);

        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to start..");
            Console.Read();

            AuthenticationResult result = null;
            int retryCount = 0;
            bool retry = false;
            do
            {
                retry = false;
                try
                {
                    result = authContext.AcquireToken(serviceResourceId, clientCredential);
                }
                catch (AdalException ex)
                {
                    if (ex.ErrorCode == "temporarily_unavailable")
                    {
                        retry = true;
                        retryCount++;
                        Thread.Sleep(3000);
                    }
                }
            } while ((retry == true) && (retryCount < 3));

            if (result == null)
            {
                Console.WriteLine("Cancelling attempt ..");
                return;
            }

            Console.WriteLine("Authenticated succesfully.. making HTTPS call..");

            MakeHttpsCall(result).Wait();

            Console.ReadLine();
            
        }

        private static async Task MakeHttpsCall(AuthenticationResult result)
        {
            string serviceBaseAddress = "https://localhost:44300/";
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            HttpResponseMessage response = await httpClient.GetAsync(serviceBaseAddress + "api/location?cityName=dc");
            
            if (response.IsSuccessStatusCode)
            {
                string r = await response.Content.ReadAsStringAsync();
                Console.WriteLine(r);
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    authContext.TokenCache.Clear();
                }
                Console.WriteLine("Access Denied!");
            }
        }
    }
}