﻿using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Urlshortener.Models;
using Urlshortener.Functions;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.WebJobs.Host;

namespace urlshortener
{
    public class PostNewUrl
    {
        public static HttpResponseMessage Run(ShortUrlRequest request, TraceWriter log)
        {
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
