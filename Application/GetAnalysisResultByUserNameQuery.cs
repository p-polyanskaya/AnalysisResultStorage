using CSharpFunctionalExtensions;
using Domain;
using MediatR;
using MongoDB.Driver;

namespace Application;

public static class GetAnalysisResultByUserNameQuery
{
    public record Request(string UserName, AnalysisResultType Type) : IRequest<IReadOnlyCollection<AnalysisResult>>;

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

            var userNameFilter = Builders<AnalysisResult>.Filter.Eq(p => p.UserName, request.UserName);
            var filter = (int)request.Type switch
            {
                0 => userNameFilter & 
                     Builders<AnalysisResult>.Filter.Eq(p => p.IsSuspiciousMessage, true),
                1 => userNameFilter & 
                     Builders<AnalysisResult>.Filter.Eq(p => p.IsSuspiciousMessage, false),
                2 => userNameFilter,
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