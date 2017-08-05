using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Urlshortener.Functions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Urlshortener.Models
{
    public class ShortUrlResponse 
    {
        internal class ShortUrlResponseObject
        {
            public string ShortUrl { get; set; }
            
            public string OriginalUrl { get; set; }

            public Dictionary<string, string> Errors { get; set; }
        }

        private object value;
        public HttpStatusCode StatusCode { get; set; }

        private string[] validUrlScheme = { "ftp", "http", "https" };

        public ShortUrlResponse(
            ShortenUrlFunctions.SaveShortUrlDelegate saveShortUrl, 
            ShortenUrlFunctions.ShortenUrlDelegate shortenUrl,
            ShortenUrlFunctions.RetrieveShortUrlDelegate retrieveShortUrl,
            ShortUrlRequest request)             
        {
            var responseObject = new ShortUrlResponseObject();

            this.value = responseObject;
            responseObject.OriginalUrl = request.OriginalUrl;

            Uri originalUrl;
            if (Uri.TryCreate(request.OriginalUrl, UriKind.Absolute, out originalUrl)
                && this.validUrlScheme.Any((scheme) => originalUrl.Scheme == scheme))
            {
                this.StatusCode = HttpStatusCode.OK;
                var shortUrl = new ShortUrl(shortenUrl, originalUrl.AbsoluteUri);
                var existingShortUrl = retrieveShortUrl(shortUrl.ShortnedUrl)
                                        .Result;
                if (existingShortUrl == null)
                {
                    saveShortUrl(shortUrl);
                }
                responseObject.ShortUrl = shortUrl.ShortnedUrl;
            }
            else
            {
                this.StatusCode = HttpStatusCode.BadRequest;
                responseObject.Errors = new Dictionary<string, string>
                {
                    {
                        "originalUrl", "The OriginalUrl field is not a valid fully-qualified http, https, or ftp URL."
                    }
                };
            }
        }
    }
}