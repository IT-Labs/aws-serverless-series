using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using ConsoleAppDynamoDB;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambdaDynamoDBAccess31
{
    public class Function
    {
        // Execution context vars
        public AmazonDynamoDBClient client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);

        public Function()
        {
            Console.WriteLine("Hello from Function Constructor");
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandlerAsync(FunctionInput input, ILambdaContext context)
        {
            Console.WriteLine("Hello from FunctionHandlerAsync");

            var postPersistenceModelService = new PostPersistenceModelService(client);
            var posts = await postPersistenceModelService.GetPostsAsync("kole.vasilevski@it-labs.com");
            posts.ForEach(item => Console.WriteLine($"Item: {item.PostUUID}, Title: {item.Title}, Comment: {item.Comment}"));

            Console.WriteLine("Good bye from FunctionHandlerAsync");

            return JsonConvert.SerializeObject(posts);
        }
    }
}
