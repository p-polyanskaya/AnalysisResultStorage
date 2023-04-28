using MongoDB.Bson.Serialization.Attributes;

namespace Domain;

public class AnalysisResultMongo
{
    [BsonId]
    public string Id { get; set; }
    public string Author { get; set; }
    public DateTime TimeOfMessage { get; set; }
    public string Text { get; set; }
    public string Source { get; set; }
    public string Topic { get; set; }
}