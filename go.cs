using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.Cosmos.Table;

namespace Company.Function
{
    public static class go
    {
        [FunctionName("go")]
        public static async  Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="{key?}")] HttpRequest req,
            string key,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string Url = System.Environment.GetEnvironmentVariable("Default404Url");

            log.LogInformation($"Short-Url-Key:{key}");

            if (!string.IsNullOrEmpty(key) && key.Length > 5)
            {

                string storageConnectionString = System.Environment.GetEnvironmentVariable("CosmosDBString");

                // Retrieve storage account information from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

                // Create a table client for interacting with the table service
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

                Console.WriteLine("Create a Table for the demo");

                // Create a table client for interacting with the table service 
                CloudTable table = tableClient.GetTableReference("short-url-keys");


                try
                {
                    var partitionKey = key.Substring(0, 3);
                    TableOperation retrieveOperation = TableOperation.Retrieve<ShortUrlEntity>(partitionKey, key);
                    TableResult result = await table.ExecuteAsync(retrieveOperation);
                    ShortUrlEntity shortKeyEntity = result.Result as ShortUrlEntity;
                    if (shortKeyEntity != null)
                    {
                        log.LogInformation("\t{0}\t{1}\t{2}\t{3}", shortKeyEntity.PartitionKey, shortKeyEntity.RowKey, shortKeyEntity.Email, shortKeyEntity.Url);
                        Url = shortKeyEntity.Url;
                    }

                    else
                    {
                        Url = System.Environment.GetEnvironmentVariable("Default404Url");
                    }

                    //if (result.RequestCharge.HasValue)
                    //{
                    //   log.LogInformation("Request Charge of Retrieve Operation: " + result.RequestCharge);
                    //}


                }
                catch (StorageException e)
                {
                    Url = System.Environment.GetEnvironmentVariable("DefaultExceptionUrl");
                }

            }
            else
            {
                Url = System.Environment.GetEnvironmentVariable("Default404Url");
            }

            var res = new HttpResponseMessage();
            res.StatusCode = HttpStatusCode.Redirect;
            res.Headers.Add("Location", Url);
            return res;

        }

    }
}
