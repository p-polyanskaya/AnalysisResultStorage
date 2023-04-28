using AnalysisResultStorage;
using Application;
using Consumers;
using GrpcServices;
using Metrics;
using Migration;
using Options;
using Prometheus;
using IMetricFactory = Prometheus.Client.IMetricFactory;

var builder = WebApplication.CreateBuilder(args);

//регистрация консьюмера
builder.Services.AddHostedService<AnalysisResultConsumer>();

//настройках данных из appsettings
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection(nameof(MongoSettings)));
builder.Services.Configure<ConsumersSettings>(builder.Configuration.GetSection(nameof(ConsumersSettings)));

//настройка монго и ее индексов
await builder.Services.SetMongo();
//настройка миграций постгреса
//builder.Services.SetPostgres();
//настрйока медиаторов
builder.Services.AddMediatR(x =>
    x.RegisterServicesFromAssemblies(typeof(GetAnalysisResultBySourceQuery.Handler).Assembly));

//регистрация сервисов
builder.Services.AddSingleton<MetricSetter>();

//настройка grpc
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

//настройка grpc
app.MapGrpcService<AnalysisResultGrpcService>();
app.MapGrpcReflectionService();

//app.Migrate();

app.UseMetricServer();

app.Run();
