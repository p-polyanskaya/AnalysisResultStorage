namespace Domain;

public class AnalysisResult
{
    public Guid Id { get; }
    public string UserName { get; }
    public DateTime TimeOfMessage { get; }
    public byte[] MessageText { get; }
    public string Source { get; }
    public bool IsSuspiciousMessage { get; }
    public byte[] AnalysisResultDescription { get; }

    public AnalysisResult(
        Guid id,
        string userName,
        byte[] messageText, 
        DateTime timeOfMessage, 
        bool isSuspiciousMessage, 
        string source,
        byte[] analysisResultDescription)
    {
        Id = id;
        UserName = userName;
        MessageText = messageText;
        TimeOfMessage = timeOfMessage;
        IsSuspiciousMessage = isSuspiciousMessage;
        Source = source;
        AnalysisResultDescription = analysisResultDescription;
    }
}