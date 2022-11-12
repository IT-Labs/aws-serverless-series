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

namespace AWSApiGatewayLambdaDynamoDB31Post
{
    public class Function
    {
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
            
            if (string.IsNullOrEmpty(request.Body))
            {
                return LambdaFunctionHelper.BadRequest("Illegal request. Body is missing.");
            }

            var post = LambdaFunctionHelper.DeserializeWith<Post>(request.Body);
            var username = post.Username;
            if (string.IsNullOrEmpty(username))
            {
                return LambdaFunctionHelper.BadRequest("Illegal request. Username does not exist.");
            }

            var title = string.IsNullOrEmpty(post.Title) ? "Basketball" : post.Title;
            var comment = string.IsNullOrEmpty(post.Comment) ? "Hello from API" : post.Comment;

            var postPersistenceModelService = new PostPersistenceModelService(client);
            var postUuid = postPersistenceModelService.AddPostAsync(username, DateTime.Now, title, comment, Guid.NewGuid().ToString()).Result;

            Console.WriteLine("Good bye from FunctionHandler");

            return LambdaFunctionHelper.OK(postUuid);
        }
    }
}
