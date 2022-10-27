// See https://aka.ms/new-console-template for more information
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using System.Reflection.Metadata;
using ConsoleAppDynamoDB;
using Amazon;


// Persistence Model
Console.WriteLine("Hello, Persistence Model!");

var postPersistenceModelService = new PostPersistenceModelService(RegionEndpoint.USEast1);

var postId = await postPersistenceModelService.AddPostAsync("some_name@mail.com", DateTime.Now, "Basketball", "Hello again", Guid.NewGuid().ToString());

var posts1 = await postPersistenceModelService.GetPostAsync("some_name@mail.com", postId);

var posts2 = await postPersistenceModelService.GetAllPostsAsync();

var posts3 = await postPersistenceModelService.GetPostsAsync("kole.vasilevski@it-labs.com");

Console.WriteLine("End Persistence Model");

// Document Model
Console.WriteLine("Hello, Document Model!");

var username = "some_cricket_player@mail.com";
var dateCreated = DateTime.Now;

var postDocumentModelService = new PostDocumentModelService();

await postDocumentModelService.AddPostAsync(username, dateCreated, "Cricket", "Hello Cricket players", Guid.NewGuid().ToString());

var posts = await postDocumentModelService.GetPostsAsync(username, dateCreated);

Console.WriteLine("End Document Model");





