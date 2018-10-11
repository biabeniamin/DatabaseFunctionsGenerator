using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public static class HttpRequestClient
    {
        public static async Task<string> GetRequest(string cmd)
        {
            return await GetRequest("http://localhost/generator/test/AccessLogs.php?cmd", cmd);
        }
        public static async Task<string> GetRequest(string url, string cmd)
        {
            var httpClient = new HttpClient();
            string response = "null";

            try
            {
                var uri = new Uri($"{url}={cmd}");
                Console.WriteLine($"Attempting to fetch data from {uri.AbsoluteUri}");
                response = await httpClient.GetStringAsync(uri);
                Console.WriteLine($"Data from {uri.AbsoluteUri} {response}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nException: {ex.Message}");
                Console.WriteLine($"\nStack Trace: {ex.StackTrace}");
            }

            return response;

        }
    }
}
