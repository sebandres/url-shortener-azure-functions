using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Urlshortener.Models;
using Urlshortener.Functions;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Host;

namespace urlshortener
{
    public class PostNewUrl
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
        {
            var content = await req.Content.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<ShortUrlRequest>(content);

            Assembly assembly = typeof(PostNewUrl).GetTypeInfo().Assembly;

            var builder = new ConfigurationBuilder()
                .AddJsonFile(new EmbeddedFileProvider(assembly, "post_new_url"), "appsettings.json", true, false);

            var configuration = builder.Build();          

            var result = new ShortUrlResponse(
                (shortUrl) => ShortenUrlFunctions.SaveShortUrl(() => configuration["StorageConnection"], shortUrl),
                ShortenUrlFunctions.ShortenUrl,
                (hash) => ShortenUrlFunctions.RetrieveShortUrl(() => configuration["StorageConnection"], hash),
                request);
            return new HttpResponseMessage
            {
                StatusCode = result.StatusCode,
                Content = new StringContent(JObject.FromObject(result.value).ToString(), Encoding.UTF8, "application/json")
            };
        }
    }
}
