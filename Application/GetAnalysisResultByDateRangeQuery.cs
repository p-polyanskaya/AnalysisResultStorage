using CSharpFunctionalExtensions;
using Domain;
using MediatR;
using MongoDB.Driver;

namespace Application;

public static class GetAnalysisResultByDateRangeQuery
{
    public record Request(DateTime Start, DateTime End, AnalysisResultType Type) : IRequest<IReadOnlyCollection<AnalysisResult>>;

    public class Handler : IRequestHandler<Request, IReadOnlyCollection<AnalysisResult>>
    {
        private readonly IMongoClient _mongoClient;

        public Handler(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        public async Task<IReadOnlyCollection<AnalysisResult>> Handle(Request request, CancellationToken cancellationToken)
        {
            var db = _mongoClient.GetDatabase("myDB10");
            var collection = db.GetCollection<AnalysisResult>("myCol");

            var dateFilter = Builders<AnalysisResult>.Filter.Gt(p => p.TimeOfMessage, request.Start) &
                             Builders<AnalysisResult>.Filter.Lt(p => p.TimeOfMessage, request.End);
            var filter = (int)request.Type switch
            {
                0 => dateFilter &
                     Builders<AnalysisResult>.Filter.Eq(p => p.IsSuspiciousMessage, true),
                1 => dateFilter &
                     Builders<AnalysisResult>.Filter.Eq(p => p.IsSuspiciousMessage, false),
                2 => dateFilter,
                _ => throw new ArgumentOutOfRangeException()
            };

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