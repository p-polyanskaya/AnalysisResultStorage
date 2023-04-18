using Application;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;

namespace GrpcServices;

public class AnalysisResultGrpcService: AnalysisResultOperation.AnalysisResultOperationBase
{
    private readonly IMediator _mediator;

    public AnalysisResultGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public override async Task<GetAnalysisResultBySourceResponse> GetAnalysisResultBySource(GetAnalysisResultBySourceRequest request, ServerCallContext context)
    {
        var query = new GetAnalysisResultBySourceQuery.Request(request.Source, ToDomainType(request.Type));
        var response = await _mediator.Send(query, context.CancellationToken);
        return new GetAnalysisResultBySourceResponse { Results = { response.Select(ToProtoAnalyseResult).ToList() } };
    }

    public override async Task<GetAnalysisResultByDateRangeResponse> GetAnalysisResultByDateRange(GetAnalysisResultByDateRangeRequest request, ServerCallContext context)
    {
        var query = new GetAnalysisResultByDateRangeQuery.Request(request.Start.ToDateTime(), request.End.ToDateTime(), ToDomainType(request.Type));
        var response = await _mediator.Send(query, context.CancellationToken);
        return new GetAnalysisResultByDateRangeResponse { Results = { response.Select(ToProtoAnalyseResult).ToList() } };
    }

    public override async Task<GetAnalysisResultByUserNameResponse> GetAnalysisResultByUserName(GetAnalysisResultByUserNameRequest request, ServerCallContext context)
    {
        var query = new GetAnalysisResultByUserNameQuery.Request(request.UserName, ToDomainType(request.Type));
        var response = await _mediator.Send(query, context.CancellationToken);
        return new GetAnalysisResultByUserNameResponse { Results = { response.Select(ToProtoAnalyseResult).ToList() } };
    }
    
    private static Domain.AnalysisResultType ToDomainType(AnalysisResultType type)
    {
        return type switch
        {
            AnalysisResultType.SuspiciousMessage => Domain.AnalysisResultType.SuspiciousMessage,
            AnalysisResultType.NotSuspiciousMessage => Domain.AnalysisResultType.NotSuspiciousMessage,
            AnalysisResultType.All => Domain.AnalysisResultType.All
        };
    }

    private static AnalyseResult ToProtoAnalyseResult(Domain.AnalysisResult analysisResult)
    {
        return new AnalyseResult
        {
            Id = analysisResult.Id.ToString(),
            AnalysisResultDescription = System.Text.Encoding.UTF8.GetString(analysisResult.AnalysisResultDescription),
            IsSuspiciousMessage = analysisResult.IsSuspiciousMessage,
            MessageText = System.Text.Encoding.UTF8.GetString(analysisResult.MessageText),
            Source = analysisResult.Source,
            TimeOfMessage = analysisResult.TimeOfMessage.ToTimestamp(),
            UserName = analysisResult.UserName
        };
    }
}