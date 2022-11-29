using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambdaTestAuthorizer
{
    public class Function
    {
        private string _arn = "arn:aws:execute-api:us-east-1:624395842874:tlbs7hb1kf/*/*/*";
        //"arn:aws:execute-api:us-east-1:624395842874:h60sfpypb2/prod/GET/posts

        private const string ApiKeyHeaderName = "Authorization";
        private const string ApiKeyHeaderNameLowerLetter = "authorization"; // request issue found on testing
        private const string AuthenticationScheme = "Bearer";


        /// <summary>
        /// FunctionHandler Authorizer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayCustomAuthorizerResponse FunctionHandler(APIGatewayCustomAuthorizerRequest request, ILambdaContext context)
        {
            var strTraceId = context.AwsRequestId;

            var authorized = false;
            _arn = request.MethodArn;

            if (!string.IsNullOrEmpty(request.AuthorizationToken) && !request.AuthorizationToken.StartsWith(AuthenticationScheme))
                return CreateResponse(authorized);

            var apikey = request.AuthorizationToken.Substring($"{AuthenticationScheme} ".Length).Trim();

            Console.WriteLine($"----- Hello from FunctionHandler Authorizer: {strTraceId}");
            Console.WriteLine($"----- request.AuthorizationToken: {request.AuthorizationToken}");
            Console.WriteLine($"----- request.MethodArn: {_arn}");
            Console.WriteLine($"----- apikey: {apikey}");

            // api key validation
            if (!string.IsNullOrEmpty(apikey))
            {
                try
                {
                    var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(apikey);
                    if (jwtSecurityToken != null)
                    {
                        // try to authorize token - do whatever you need
                        authorized = true;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Authorization started.{Environment.NewLine}TraceId: {strTraceId}");
                    authorized = false;
                }
            }

            Console.WriteLine($"----- authorized: {authorized}");

            return CreateResponse(authorized);
        }

        private APIGatewayCustomAuthorizerResponse CreateResponse(bool authorized)
        {
            APIGatewayCustomAuthorizerPolicy policy = new APIGatewayCustomAuthorizerPolicy
            {
                Version = "2012-10-17",
                Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>()
            };

            policy.Statement.Add(new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement
            {
                Action = new HashSet<string>(new string[] { "execute-api:Invoke" }),
                Effect = authorized ? "Allow" : "Deny",
                Resource = new HashSet<string>(new string[] { _arn })

            });

            APIGatewayCustomAuthorizerContextOutput contextOutput = new APIGatewayCustomAuthorizerContextOutput
            {
                ["User"] = "User",
                ["Path"] = _arn
            };

            var response = new APIGatewayCustomAuthorizerResponse
            {
                PrincipalID = "User",
                Context = contextOutput,
                PolicyDocument = policy
            };

            var jsonString = JsonConvert.SerializeObject(response);
            Console.WriteLine($"----- Response: {jsonString}");

            return response;
        }

    }
}
