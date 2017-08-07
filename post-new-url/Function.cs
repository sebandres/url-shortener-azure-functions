using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace urlshortener
{
    public class PostNewUrl
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, string name)
        {
            Assembly assembly = typeof(PostNewUrl).GetTypeInfo().Assembly;

            var builder = new ConfigurationBuilder()
                .AddJsonFile(new EmbeddedFileProvider(assembly, "post_new_url"), "appsettings.json", true, false);
                //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            var configuration = builder.Build();

            // parse query parameter
            /*string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;
                */
            // Get request body
            //var data = await req.Content.ReadAsStringAsync();

            // Set name to query string or body data
            //name = name ?? data;

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, $"Hello you {name} {configuration["Values:test"]}");
        }
    }
}
