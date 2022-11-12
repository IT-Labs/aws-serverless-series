using Amazon.DynamoDBv2.DataModel;
using System;

namespace ConsoleAppDynamoDB
{
    [DynamoDBTable("Posts")]
    public class Post
    {
        [DynamoDBHashKey]
        public string Username { get; set; }

        [DynamoDBRangeKey]
        public DateTime DateCreated { get; set; }

        public string Title { get; set; }

        public string Comment { get; set; }

        public string PostUUID { get; set; }
    }
}
