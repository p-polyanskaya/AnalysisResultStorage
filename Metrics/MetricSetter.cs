using Counter = Prometheus.Counter;

namespace Metrics;

public class MetricSetter
{
    private readonly Counter _newsByTopic;
    
    public MetricSetter()
    {
        _newsByTopic = Prometheus.Metrics.CreateCounter(
            "number_of_news_by_topic",
            "Число новостей по теме",
            "topic");
    }

    public void IncNewsCountByTopic(string topic)
    {
        _newsByTopic.WithLabels(topic).Inc();
    }
}