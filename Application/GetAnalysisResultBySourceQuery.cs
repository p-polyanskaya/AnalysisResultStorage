using Domain;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Options;

namespace Application;

public static class GetAnalysisResultBySourceQuery
{
    public record Request(string Source) : IRequest<IReadOnlyCollection<AnalysisResultMongo>>;

    public class Handler : IRequestHandler<Request, IReadOnlyCollection<AnalysisResultMongo>>
    {
        private readonly IMongoClient _mongoClient;
        private readonly IOptions<MongoSettings> _mongoSettings;

        public Handler(IMongoClient mongoClient, IOptions<MongoSettings> mongoSettings)
        {
            _mongoClient = mongoClient;
            _mongoSettings = mongoSettings;
        }
        
        public async Task<IReadOnlyCollection<AnalysisResultMongo>> Handle(Request request, CancellationToken cancellationToken)
        {
            var db = _mongoClient.GetDatabase(_mongoSettings.Value.DataBase);
            var collection = db.GetCollection<AnalysisResultMongo>(_mongoSettings.Value.Collection);

            var filter = Builders<AnalysisResultMongo>.Filter.Eq(p => p.Source, request.Source);

            var res = await collection
                .Find(filter)
                .ToListAsync(cancellationToken: cancellationToken);

            if (res is null)
            {
                throw new Exception("Произошла ошибка при получении данных из бд");
            }

            return res;
        }
    }
}