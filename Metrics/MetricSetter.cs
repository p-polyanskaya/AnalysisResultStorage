using Counter = Prometheus.Counter;

namespace Metrics;

public class MetricSetter
{
    private readonly Counter _suspiciousMessagesBySource;
    
    public MetricSetter()
    {
        _suspiciousMessagesBySource = Prometheus.Metrics.CreateCounter(
            "number_of_news_by_source",
            "Число новостей по источнику",
            "source");
    }

    public void IncSuspiciousMessageCount(string source)
    {
        _suspiciousMessagesBySource.WithLabels(source).Inc();
    }
}