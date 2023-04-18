using Application;
using Confluent.Kafka;
using Domain;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Consumers;

public class AnalysisResultConsumer: BackgroundService
{
    private readonly ConsumerBuilder<Ignore, AnalysisResult> _builder;
    private readonly IMediator _mediator;
    private const string Topic = "analyses_results";
    private const string GroupId = "analyses_result_storage_dev";
    private const string BootstrapServers = "localhost:9092";
    
    public AnalysisResultConsumer(IMediator mediator)
    {
        _mediator = mediator;
        var config = new ConsumerConfig
        {
            BootstrapServers = BootstrapServers,
            GroupId = GroupId,
            EnableAutoCommit = false
        };
        
        _builder = new ConsumerBuilder<Ignore, AnalysisResult>(config)
            .SetValueDeserializer(new EventDeserializer<AnalysisResult>());
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        using var consumer = _builder.Build();
        consumer.Subscribe(Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);

            var request = new AnalysisResultSaveCommand.Request(consumeResult.Message.Value);
            await _mediator.Send(request, stoppingToken);
            consumer.Commit(consumeResult);
        }

        consumer.Close();
    }
}