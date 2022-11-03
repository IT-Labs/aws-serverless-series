using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleAppLambdaDynamoDB31
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"Start exec ConsoleAppLambdaDynamoDB31");

            var lambdaClient = new AmazonLambdaClient(RegionEndpoint.USEast1);

            var lambdaRequest = new InvokeRequest
            {
                FunctionName = "AWSLambdaDynamoDBAccess31",
                Payload = "{ \"username\": \"kole.vasilevski@it-labs.com\" }" // json representation
            };

            try
            {
                var response = await lambdaClient.InvokeAsync(lambdaRequest);
                if (response != null)
                {
                    using (var sr = new StreamReader(response.Payload))
                    {
                        var result = await sr.ReadToEndAsync();
                        Console.WriteLine($"Result: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exec ConsoleAppLambdaDynamoDB31. {System.Environment.NewLine}Error: {ex}");
                throw;
            }

            Console.WriteLine($"End exec ConsoleAppLambdaDynamoDB31");
        }
    }
}
