namespace SentimentChatbotWebAPI.Interfaces
{
    public interface IAzureSecretClientWrapper
    {
        string GetSecret(string secretName);

    }
}