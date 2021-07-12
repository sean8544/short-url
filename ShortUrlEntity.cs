using System;
using Microsoft.Azure.Cosmos.Table;

namespace Company.Function
{
    public class ShortUrlEntity:TableEntity
    {
        public ShortUrlEntity()
        {
        }

        public ShortUrlEntity(string key)
        {
            PartitionKey = key.Substring(0,3);
            RowKey = key;
        }

        public string Email { get; set; }

        public string Url { get; set; }
    }
}
