namespace Urlshortener.Models
{
    using System;
    using Urlshortener.Functions;
    using Microsoft.WindowsAzure.Storage.Table;

    internal class ShortUrlHit : TableEntity
    {
        public string ShortUrl { get; set; }
        
        public DateTime CreatedDateTime { get; set; }

        public ShortUrlHit(string shortUrl)
        {
            this.CreatedDateTime = DateTime.UtcNow;
            this.ShortUrl = shortUrl;
            this.RowKey = this.CreatedDateTime.Ticks.ToString();
            this.PartitionKey = string.Format("ShortUrlHit{0}", this.ShortUrl);
        }

        private ShortUrlHit() 
        { 
        }
    }
}