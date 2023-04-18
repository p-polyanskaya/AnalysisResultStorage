using Application;
using Consumers;
using Domain;
using GrpcServices;
using MongoDB.Driver;
using FluentMigrator.Runner;
using Migration;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddHostedService<AnalysisResultConsumer>();
builder.Services.AddMediatR(x =>
    x.RegisterServicesFromAssemblies(typeof(GetAnalysisResultBySourceQuery.Handler).Assembly));

var mongoClient = new MongoClient("mongodb://localhost:27017");
var db = mongoClient.GetDatabase("myDB11");
var collection = db.GetCollection<AnalysisResult>("myCol");
var keys = Builders<AnalysisResult>.IndexKeys.Ascending(x => x.Id);
await collection.Indexes.CreateOneAsync(new CreateIndexModel<AnalysisResult>(keys));
//var ixList = collection.Indexes.List().ToList<BsonDocument>();
//ixList.ForEach(ix => Console.WriteLine(ix));

builder.Services.AddSingleton<IMongoClient>(s => mongoClient);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services
    .AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(
            "Server=127.0.0.1;Port=5432;Userid=postgres;Password=postgres;Database=course_db")
        .ScanIn(typeof(CreatePostgresTable).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

var app = builder.Build();
app.MapGrpcService<AnalysisResultGrpcService>();
app.MapGrpcReflectionService();

app.Migrate();

app.Run();