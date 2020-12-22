using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            var tokenRequest = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "console-client",
                ClientSecret = "console-secret",
                Scope = "api1"
            });

            if (tokenRequest.IsError)
            {
                Console.WriteLine(tokenRequest.Error);
                return;
            }

            Console.WriteLine(tokenRequest.Json);


            var api = new HttpClient();
            api.SetBearerToken(tokenRequest.AccessToken);

            var response = await api.GetAsync("https://localhost:6001/api/identity");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return;
            }
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("------------");
            Console.WriteLine(content);
            Console.WriteLine("**************");
            Console.WriteLine(JArray.Parse(content));
            Console.ReadLine();
        }
    }
}
