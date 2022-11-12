using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ConsoleAppDynamoDB
{
    public static class LambdaFunctionHelper
    {
        public static APIGatewayHttpApiV2ProxyResponse NotFound()
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Body = string.Empty
            };
        }

        public static APIGatewayHttpApiV2ProxyResponse NotFound(object obj)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Body = SerializeWith(obj)
            };
        }

        public static APIGatewayHttpApiV2ProxyResponse BadRequest(object obj)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Headers = new Dictionary<string, string> {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" }
                },
                Body = SerializeWith(obj)
            };
        }

        public static APIGatewayHttpApiV2ProxyResponse BadRequest(string message)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Headers = new Dictionary<string, string> {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" }
                },
                Body = message
            };
        }

        public static APIGatewayHttpApiV2ProxyResponse InternalServerError(object obj)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Headers = new Dictionary<string, string> {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" }
                },
                Body = SerializeWith(obj)
            };
        }

        public static APIGatewayHttpApiV2ProxyResponse OK()
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = string.Empty
            };
        }

        public static APIGatewayHttpApiV2ProxyResponse OK(object obj)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = SerializeWith(obj)
            };
        }

        public static string SerializeWith<T>(T value)
        {
            using var buffer = new MemoryStream();
            var jsonSerializer = new DefaultLambdaJsonSerializer();
            jsonSerializer.Serialize<T>(value, buffer); ;
            return Encoding.UTF8.GetString(buffer.ToArray());
        }

        public static T DeserializeWith<T>(string value)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(value);
            MemoryStream stream = new MemoryStream(byteArray);
            var jsonSerializer = new DefaultLambdaJsonSerializer();
            return jsonSerializer.Deserialize<T>(stream); ;
        }
    }
}
