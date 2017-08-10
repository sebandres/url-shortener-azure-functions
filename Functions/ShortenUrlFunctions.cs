

namespace Urlshortener.Functions
{
    using System;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Urlshortener.Models;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public static class ShortenUrlFunctions
    {
        public delegate string GetConnectionStringDelegate();
        public delegate string ShortenUrlDelegate(string originalUrl);
        public delegate Task<string> RetrieveShortUrlDelegate(string hash);
        public delegate Task<bool> SaveShortUrlDelegate(ShortUrl url);

        internal static CloudStorageAccount GenerateStorageAccount(GetConnectionStringDelegate connectionString)
        {
            return CloudStorageAccount
               .Parse(connectionString());
        }

        internal static CloudTable GenerateCloudTable(CloudStorageAccount storageAccount, string tableName)
        {
            return storageAccount
                .CreateCloudTableClient()
                .GetTableReference(tableName);
        }

        public static string ShortenUrl(string originalUrl)
        {
            long h = 0;
            for (int i = 0; i < originalUrl.Length; i++)
            {
                h += originalUrl[i] * 31 ^ originalUrl.Length - (i + 1);
            }
            return h.ToString("x");
        }

        public static bool ValidateShortUrl(GetConnectionStringDelegate connectionString, string hash)
        {
            return RetrieveShortUrl(connectionString, hash) != null;
        }

        public static async Task<string> RetrieveShortUrl(GetConnectionStringDelegate connectionString, string hash)
        {
            var res = await GenerateCloudTable(GenerateStorageAccount(connectionString), "ShortUrl")
                .ExecuteAsync(TableOperation.Retrieve<ShortUrl>("ShortUrl", hash));

            if (res.Result == null)
            {
                return null;
            }
            else
            {
                return ((ShortUrl)res.Result).OriginalUrl;
            }
        }

        public static async Task<TableResult> RecordShortUrlHit(GetConnectionStringDelegate connectionString, string hash)
        {
            var table = GenerateCloudTable(GenerateStorageAccount(connectionString), "ShortUrlHit");
            await table.CreateIfNotExistsAsync();
            return await table.ExecuteAsync(TableOperation.InsertOrReplace(new ShortUrlHit(hash)));
        }

        public static async Task<bool> SaveShortUrl(GetConnectionStringDelegate getConnectionString, ShortUrl url)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount
                .Parse(getConnectionString());

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("ShortUrl");

            await table.CreateIfNotExistsAsync();

            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(url);

            // Execute the insert operation.
            var tableOperationResult = await table.ExecuteAsync(insertOperation);

            return tableOperationResult.HttpStatusCode == (int)HttpStatusCode.OK;
        }
    }
}