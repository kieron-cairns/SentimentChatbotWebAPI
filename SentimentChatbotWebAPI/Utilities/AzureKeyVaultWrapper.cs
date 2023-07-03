using Azure.Security.KeyVault.Secrets;
using SentimentChatbotWebAPI.Interfaces;

namespace SentimentChatbotWebAPI.Utilities
{
    public class AzureKeyVaultWrapper : IAzureKeyVaultWrapper
    {
        private readonly SecretClient _secretClient;

        public AzureKeyVaultWrapper(SecretClient secretClient)
        {
            _secretClient = secretClient;
        }

        public string GetSecret(string secretName)
        {
            KeyVaultSecret secret = _secretClient.GetSecret(secretName);

            return secret.Value;
        }
    }
}
