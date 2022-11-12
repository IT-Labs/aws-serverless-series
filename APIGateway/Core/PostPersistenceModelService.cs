using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon;
using Amazon.DynamoDBv2.DocumentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ConsoleAppDynamoDB
{
    public class PostPersistenceModelService
    {
        private readonly DynamoDBContext _dynamoDBContext;

        public PostPersistenceModelService()
        {
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            _dynamoDBContext = new DynamoDBContext(new AmazonDynamoDBClient(RegionEndpoint.USEast1), config);
        }

        public PostPersistenceModelService(RegionEndpoint regionEndpoint)
        {
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            _dynamoDBContext = new DynamoDBContext(new AmazonDynamoDBClient(regionEndpoint), config);
        }

        public PostPersistenceModelService(AmazonDynamoDBClient client)
        {
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            _dynamoDBContext = new DynamoDBContext(client, config);
        }

        public async Task<List<Post>> GetAllPostsAsync()
        {
            var search = _dynamoDBContext.ScanAsync<Post>(null);
            return await search.GetNextSetAsync();
        }

        public async Task<List<Post>> GetPostsAsync(string username)
        {
            var items = new List<Post>();
            AsyncSearch<Post> query = _dynamoDBContext.QueryAsync<Post>(username);

            while (query.IsDone == false)
            {
                items.AddRange(await query.GetNextSetAsync());
            }

            return items;
        }

        public async Task<Post?> GetPostAsync(string username, string PostUUID)
        {
            var config = new DynamoDBOperationConfig
            {
                QueryFilter = new List<ScanCondition> {
                    new ScanCondition(nameof(Post.PostUUID), ScanOperator.Equal, PostUUID)
                }
            };
            var posts = await _dynamoDBContext.QueryAsync<Post>(username, config).GetRemainingAsync();
            return posts.FirstOrDefault();
        }

        public async Task<string> AddPostAsync(string username, DateTime dateCreated, string title, string comment, string postUUID)
        {
            var post = new Post
            {
                Username = username,
                DateCreated = dateCreated,
                Title = title,
                Comment = comment,
                PostUUID = postUUID
            };

            await _dynamoDBContext.SaveAsync<Post>(post);
            return postUUID;
        }

        public async Task DeletePostsAsync(string username, string PostUUID)
        {
            var post = GetPostAsync(username, PostUUID);
            await _dynamoDBContext.DeleteAsync<Post>(post);
        }
    }

}
