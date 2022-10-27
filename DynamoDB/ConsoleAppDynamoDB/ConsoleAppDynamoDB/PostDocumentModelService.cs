using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;
using Amazon.DynamoDBv2.DocumentModel;
using System.ComponentModel.Design;
using Amazon.Runtime.Internal;
using System.Reflection.Metadata;
using Document = Amazon.DynamoDBv2.DocumentModel.Document;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ConsoleAppDynamoDB
{
    // .NET: Object persistence model
    public class PostDocumentModelService
    {
        private readonly AmazonDynamoDBClient _client;
        private const string TableName = "Posts";

        public PostDocumentModelService()
        {
            _client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
        }

        public PostDocumentModelService(RegionEndpoint regionEndpoint)
        {
            _client = new AmazonDynamoDBClient(regionEndpoint);
        }

        public async Task<Document> GetPostsAsync(string username, DateTime dateCreated)
        {
            Table postsTable = Table.LoadTable(_client, TableName);

            var post = new Document();
            post["Username"] = username;
            post["DateCreated"] = dateCreated;

            Document postsDocument = await postsTable.GetItemAsync(post);

            return postsDocument;
        }

        public async Task<Document> AddPostAsync(string username, DateTime dateCreated, string title, string comment, string postUUID)
        {
            Table table = Table.LoadTable(_client, TableName);

            var post = new Document();
            post["Username"] = username;
            post["DateCreated"] = dateCreated;
            post["Title"] = title;
            post["Comment"] = comment;
            post["PostUUID"] = postUUID;
            post["OtherData"] = "Something";
            await table.PutItemAsync(post);

            return post;
        }
    }

}
