using Domain;
using MediatR;
using Metrics;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Options;

namespace Application;

public static class AnalysisResultSaveCommand
{
    public record Request(AnalysisResult AnalysisResult) : IRequest<Unit>;

    public class Handler : IRequestHandler<Request, Unit>
    {
        private readonly IMongoClient _mongoClient;
        private readonly MetricSetter _metricSetter;
        private readonly IOptions<MongoSettings> _mongoSettings;

        public Handler(IMongoClient mongoClient, MetricSetter metricSetter, IOptions<MongoSettings> mongoSettings)
        {
            _mongoClient = mongoClient;
            _metricSetter = metricSetter;
            _mongoSettings = mongoSettings;
        }
        
        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var db = _mongoClient.GetDatabase(_mongoSettings.Value.DataBase);
            var collection = db.GetCollection<AnalysisResultMongo>(_mongoSettings.Value.Collection);

            var analysisResult = new AnalysisResultMongo
            {
                Author = request.AnalysisResult.Message.Author,
                Id = request.AnalysisResult.Message.Id.ToString(),
                Source = request.AnalysisResult.Message.Source,
                Text = request.AnalysisResult.Message.Text,
                TimeOfMessage = request.AnalysisResult.Message.TimeOfMessage,
                Topic = request.AnalysisResult.Topic
            };
            
            try
            {
                await collection.InsertOneAsync(analysisResult, new InsertOneOptions(), cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            _metricSetter.IncSuspiciousMessageCount("source");//request.AnalysisResult.Source);
            

            return Unit.Value;
        }
    }
}