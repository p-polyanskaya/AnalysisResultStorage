using Application;
using Confluent.Kafka;
using Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Options;

namespace Consumers;

public class AnalysisResultConsumer: BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerBuilder<Ignore, AnalysisResult> _builder;
    private readonly IOptions<ConsumersSettings> _consumersSettings;
    
    public AnalysisResultConsumer(IOptions<ConsumersSettings> consumersSettings, IServiceProvider serviceProvider)
    {
        _consumersSettings = consumersSettings;
        _serviceProvider = serviceProvider;
        var config = new ConsumerConfig
        {
            BootstrapServers = _consumersSettings.Value.ConsumerForAnalyzedMessages.BootstrapServers,
            GroupId = _consumersSettings.Value.ConsumerForAnalyzedMessages.GroupId,
            EnableAutoCommit = false
        };
        
        _builder = new ConsumerBuilder<Ignore, AnalysisResult>(config)
            .SetValueDeserializer(new EventDeserializer<AnalysisResult>());
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    { 
        await Task.Yield();

        using var consumer = _builder.Build();
        consumer.Subscribe(_consumersSettings.Value.ConsumerForAnalyzedMessages.Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var request = new AnalysisResultSaveCommand.Request(consumeResult.Message.Value);
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(request, stoppingToken);
                consumer.Commit(consumeResult);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        consumer.Close();
    }
}