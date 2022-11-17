using System;

namespace AWSApiGatewayLambdaAurora31
{
    public class Post
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
