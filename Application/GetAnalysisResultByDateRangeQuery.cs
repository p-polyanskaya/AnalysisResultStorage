using Domain;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Options;

namespace Application;

public static class GetAnalysisResultByDateRangeQuery
{
    public record Request(DateTime Start, DateTime End, string? Topic) : IRequest<IReadOnlyCollection<AnalysisResultMongo>>;

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

            var filter = Builders<AnalysisResultMongo>.Filter.Gt(p => p.TimeOfMessage, request.Start) &
                             Builders<AnalysisResultMongo>.Filter.Lt(p => p.TimeOfMessage, request.End);

            if (request.Topic != "")
            {
                filter &= Builders<AnalysisResultMongo>.Filter.Eq(p => p.Topic, request.Topic);
            }

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