using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSApiGatewayLambdaAurora31
{
    public class Functions
    {
        // This const is the name of the environment variable that the serverless.template will use to set
        // the name of the DynamoDB table used to store Post posts.
        public const string ID_QUERY_STRING_NAME = "Id";
        public const string CONNECTION_STRING = "server=aurora-db-test-cluster.cluster-ciu62km82pwn.us-east-1.rds.amazonaws.com;database=social_network;user=admin;password=admin123";

        // Execution context
        public AuroraContext dbContext = new AuroraContext(CONNECTION_STRING);

        /// <summary>
        /// Default constructor that Lambda will invoke
        /// </summary>
        public Functions()
        {
        }

        /// <summary>
        /// A Lambda function that returns back a page worth of Post posts
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The list of Posts</returns>
        public async Task<APIGatewayHttpApiV2ProxyResponse> GetPostsAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Getting Posts");

            var posts = dbContext.Posts.ToList();

            return LambdaFunctionHelper.OK(posts);
        }

        /// <summary>
        /// A Lambda function that returns the Post identified by PostId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<APIGatewayHttpApiV2ProxyResponse> GetPostAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            string postParamId = null;
            if (request.PathParameters != null && request.PathParameters.ContainsKey(ID_QUERY_STRING_NAME))
                postParamId = request.PathParameters[ID_QUERY_STRING_NAME];
            else if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey(ID_QUERY_STRING_NAME))
                postParamId = request.QueryStringParameters[ID_QUERY_STRING_NAME];

            int.TryParse(postParamId, out int postId);

            if (postId < 0)
            {
                return LambdaFunctionHelper.BadRequest($"Missing required parameter {ID_QUERY_STRING_NAME}");
            }

            context.Logger.LogLine($"Getting Post {postId}");

            var post = dbContext.Posts.Where(x => x.Id == postId).FirstOrDefault();

            context.Logger.LogLine($"Found Post: {post != null}");

            if (post == null)
            {
                return LambdaFunctionHelper.NotFound();
            }

            return LambdaFunctionHelper.OK(post);
        }

        /// <summary>
        /// A Lambda function that adds a Post post
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<APIGatewayHttpApiV2ProxyResponse> AddPostAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            var post = LambdaFunctionHelper.DeserializeWith<Post>(request?.Body);
            post.DateCreated = DateTime.Now;

            dbContext.Posts.Add(post);
            dbContext.SaveChanges();

            return LambdaFunctionHelper.OK(post.Id.ToString());
        }

        /// <summary>
        /// A Lambda function that removes a Post post
        /// </summary>
        /// <param name="request"></param>
        public async Task<APIGatewayHttpApiV2ProxyResponse> RemovePostAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            string postParamId = null;
            if (request.PathParameters != null && request.PathParameters.ContainsKey(ID_QUERY_STRING_NAME))
                postParamId = request.PathParameters[ID_QUERY_STRING_NAME];
            else if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey(ID_QUERY_STRING_NAME))
                postParamId = request.QueryStringParameters[ID_QUERY_STRING_NAME];

            int.TryParse(postParamId, out int postId);

            if (postId < 0)
            {
                return LambdaFunctionHelper.BadRequest($"Missing required parameter {ID_QUERY_STRING_NAME}");
            }

            context.Logger.LogLine($"Getting Post {postId}");

            var post = dbContext.Posts.Where(x => x.Id == postId).FirstOrDefault();

            context.Logger.LogLine($"Found Post: {post != null}");

            if (post == null)
            {
                return LambdaFunctionHelper.NotFound();
            }

            context.Logger.LogLine($"Deleting Post with Id {postId}");

            dbContext.Posts.Remove(post);
            dbContext.SaveChanges();

            return LambdaFunctionHelper.OK();
        }
    }
}
