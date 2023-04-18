using Domain;
using MediatR;
using MongoDB.Driver;

namespace Application;

public static class AnalysisResultSaveCommand
{
    public record Request(AnalysisResult AnalysisResult) : IRequest<Unit>;

    public class Handler : IRequestHandler<Request, Unit>
    {
        private readonly IMongoClient _mongoClient;

        public Handler(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }
        
        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var db = _mongoClient.GetDatabase("myDB10");
            var collection = db.GetCollection<AnalysisResult>("myCol");

            try
            {
                await collection.InsertOneAsync(request.AnalysisResult, new InsertOneOptions(), cancellationToken);
            }
            catch (Exception ex)
            {
                
            }

            return Unit.Value;
        }
    }
}