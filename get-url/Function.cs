using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Urlshortener.Functions;

namespace urlshortener
{
    public class GetUrl
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, string hash, TraceWriter log)
        {
            Assembly assembly = typeof(GetUrl).GetTypeInfo().Assembly;

            var builder = new ConfigurationBuilder()
                .AddJsonFile(new EmbeddedFileProvider(assembly, "post_new_url"), "appsettings.json", true, false);

            var configuration = builder.Build();

            var shortUrl = await ShortenUrlFunctions
                .RetrieveShortUrl(
                    () => configuration["StorageConnection"],
                    hash);

            if (shortUrl == null)
            {
                return new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }
            else
            {
                var response = new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.Redirect,
                };
                response.Headers.Location = new System.Uri(shortUrl);
                return response;
            }
        }
    }
}
