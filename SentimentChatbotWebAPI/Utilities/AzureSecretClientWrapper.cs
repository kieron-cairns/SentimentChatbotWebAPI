using SentimentChatbotWebAPI.Interfaces;

namespace SentimentChatbotWebAPI.Utilities
{
    public class AzureSecretClientWrapper : IAzureSecretClientWrapper
    {
        private readonly IAzureKeyVaultWrapper _azureKeyVaultWrapper;

        public AzureSecretClientWrapper(IAzureKeyVaultWrapper azureKeyVaultWrapper)
        {
            _azureKeyVaultWrapper = azureKeyVaultWrapper;
        }

        public string GetSecret(string secretName)
        {
            string secret = _azureKeyVaultWrapper.GetSecret(secretName);
            return secret;

        }
    }
}
