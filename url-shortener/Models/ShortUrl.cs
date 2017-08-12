namespace Urlshortener.Models
{
    using System;
    using Urlshortener.Functions;
    using Microsoft.WindowsAzure.Storage.Table;

    public class ShortUrl : TableEntity
    {
        public string ShortnedUrl { get; set; }

        public string OriginalUrl { get; set; }

        public int HitCount { get; set; }

        public ShortUrl()
        {
        }

        public ShortUrl(ShortenUrlFunctions.ShortenUrlDelegate shortenUrl, string originalUrl)
        {
            this.OriginalUrl = originalUrl;
            this.ShortnedUrl = shortenUrl(originalUrl);
            this.RowKey = this.ShortnedUrl;
            this.PartitionKey = "ShortUrl";
            this.HitCount = 0;
        }
    }
}