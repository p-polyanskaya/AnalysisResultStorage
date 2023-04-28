using Domain;
using FluentMigrator.Runner;
using Microsoft.Extensions.Options;
using Migration;
using MongoDB.Driver;
using Options;

namespace AnalysisResultStorage;

public static class DependencyInjection
{
    public static async Task SetMongo(this IServiceCollection services)
    {
        var options = services.BuildServiceProvider().GetService<IOptions<MongoSettings>>();
        var mongoClient = new MongoClient(options!.Value.Url);
        var db = mongoClient.GetDatabase(options.Value.DataBase);
        var collection = db.GetCollection<AnalysisResultMongo>(options.Value.Collection);
        var keys = Builders<AnalysisResultMongo>.IndexKeys.Ascending(x => x.Id);
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<AnalysisResultMongo>(keys));
    
        services.AddSingleton<IMongoClient>(s => mongoClient);
    }

    public static void SetPostgres(this IServiceCollection services)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(
                    "Server=127.0.0.1;Port=5432;Userid=postgres;Password=postgres;Database=course_db")
                .ScanIn(typeof(CreatePostgresTable).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());
    }
}