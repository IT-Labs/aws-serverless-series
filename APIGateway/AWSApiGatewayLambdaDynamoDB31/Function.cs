using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ConsoleAppDynamoDB;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSApiGatewayLambdaDynamoDB31
{
    public class Function
    {
        // Execution context vars
        public AmazonDynamoDBClient client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayHttpApiV2ProxyResponse FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            Console.WriteLine("Hello from FunctionHandler");

            if (request.QueryStringParameters == null)
            {
                return LambdaFunctionHelper.BadRequest("Add username as QueryStringParameter");
            }

            if (!request.QueryStringParameters.TryGetValue("username", out string username))
            {
                return LambdaFunctionHelper.BadRequest("Username as QueryStringParameter does not exist");
            }

            var postPersistenceModelService = new PostPersistenceModelService(client);
            var posts = postPersistenceModelService.GetPostsAsync(username).Result;

            posts.ForEach(item => Console.WriteLine($"Item: {item.PostUUID}, Title: {item.Title}, Comment: {item.Comment}"));
            Console.WriteLine("Good bye from FunctionHandler");

            return LambdaFunctionHelper.OK(posts);
        }
    }
}
