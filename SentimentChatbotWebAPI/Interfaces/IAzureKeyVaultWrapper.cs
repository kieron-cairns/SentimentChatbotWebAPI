namespace SentimentChatbotWebAPI.Interfaces
{
    public interface IAzureKeyVaultWrapper
    {
        string GetSecret(string secretName);
    }
}