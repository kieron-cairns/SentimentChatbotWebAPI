using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SentimentChatbotWebAPI.Interfaces;

namespace SentimentChatbotWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IChatbotRepository _repository;
        private readonly IAzureSecretClientWrapper _azureSecretClientWrapper;
        private readonly IConfiguration _configuration;

        public ChatbotController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IChatbotRepository repository, IAzureSecretClientWrapper azureSecretClientWrapper, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            _azureSecretClientWrapper = azureSecretClientWrapper;
            _configuration = configuration;
        }

    }
}
