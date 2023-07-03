using Moq;
using SentimentChatbotWebAPI.Interfaces;

namespace SentimentChatbotWebAPITests
{
    public class ControllerMethodTests
    {

        private readonly Mock<IAzureSecretClientWrapper> _azureSecretClientWrapper;

        public ControllerMethodTests()
        {
            _azureSecretClientWrapper = new Mock<IAzureSecretClientWrapper>();
        }

        [Fact]
        public void Test1()
        {

        }
    }
}