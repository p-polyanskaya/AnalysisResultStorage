using MongoDB.Bson.Serialization.Attributes;

namespace Domain;

public class AnalysisResult
{
    [BsonId]
    public string Id { get; set; }
    public Message Message { get; }
    public string Topic { get; }

    public AnalysisResult(
        Message message,
        string topic)
    {
        Message = message;
        Topic = topic;
    }
}