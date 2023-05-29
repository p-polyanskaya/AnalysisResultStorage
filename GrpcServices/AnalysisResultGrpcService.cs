using Application;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;

namespace GrpcServices;

public class AnalysisResultGrpcService : AnalysisResultOperation.AnalysisResultOperationBase
{
    private readonly IMediator _mediator;

    public AnalysisResultGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GetAnalysisResultBySourceResponse> GetAnalysisResultBySource(
        GetAnalysisResultBySourceRequest request, ServerCallContext context)
    {
        var query = new GetAnalysisResultBySourceQuery.Request(request.Source);
        var response = await _mediator.Send(query, context.CancellationToken);
        Console.WriteLine(response.Count);
        return new GetAnalysisResultBySourceResponse { Results = { response.Select(ToProtoAnalyseResult).ToList().Take(500) } };
    }

    public override async Task<GetAnalysisResultByDateRangeResponse> GetAnalysisResultByDateRange(
        GetAnalysisResultByDateRangeRequest request, ServerCallContext context)
    {
        var query = new GetAnalysisResultByDateRangeQuery.Request(request.Start.ToDateTime(), request.End.ToDateTime(),
            request.Topic);
        var response = await _mediator.Send(query, context.CancellationToken);
        return new GetAnalysisResultByDateRangeResponse
            { Results = { response.Select(ToProtoAnalyseResult).ToList().Take(500) } };
    }

    public override async Task<GetAnalysisResultByAuthorResponse> GetAnalysisResultByAuthor(
        GetAnalysisResultByAuthorRequest request, ServerCallContext context)
    {
        var query = new GetAnalysisResultByAuthorQuery.Request(request.Author);
        var response = await _mediator.Send(query, context.CancellationToken);
        return new GetAnalysisResultByAuthorResponse { Results = { response.Select(ToProtoAnalyseResult).ToList().Take(500) } };
    }

    private static AnalyseResult ToProtoAnalyseResult(Domain.AnalysisResultMongo analysisResult)
    {
        return new AnalyseResult
        {
            Id = analysisResult.Id,
            Text = analysisResult.Text,
            Source = analysisResult.Source,
            TimeOfMessage = analysisResult.TimeOfMessage.ToTimestamp(),
            Author = analysisResult.Author,
            Topic = analysisResult.Topic
        };
    }
}