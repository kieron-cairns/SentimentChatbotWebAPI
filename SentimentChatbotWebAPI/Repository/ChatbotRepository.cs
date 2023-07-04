using SentimentChatbotWebAPI.Interfaces;

namespace SentimentChatbotWebAPI.Repository
{
    public class ChatbotRepository : IChatbotRepository
    {
        private readonly ISentimentHistoryContext _context;
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly IAzureSecretClientWrapper _azureSecretClientWrapper;


        public ChatbotRepository(ISentimentHistoryContext context, IConfiguration configuration, IJwtTokenHandler jwtTokenHandler, IAzureSecretClientWrapper azureSecretClientWrapper)
        {
            _context = context;
            _configuration = configuration;
            _jwtTokenHandler = jwtTokenHandler;
            _azureSecretClientWrapper = azureSecretClientWrapper;
        }
    }
}
