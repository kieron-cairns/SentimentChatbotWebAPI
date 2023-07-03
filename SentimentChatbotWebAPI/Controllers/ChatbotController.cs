using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SentimentChatbotWebAPI.Interfaces;
using SentimentChatbotWebAPI.Models;

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

        [HttpPost("/AuthenticateUser")]
        public IActionResult AuthenticateUser()
        {
            string username = Request.Headers["username"].FirstOrDefault();
            string password = Request.Headers["password"].FirstOrDefault();

            User user = new User
            {
                Username = username,
                Password = password,
                Role = "User"
            };
            try
            {
                if (username == _azureSecretClientWrapper.GetSecret("SentimentLoginName") && password == _azureSecretClientWrapper.GetSecret("SentimentUserPassword"))
                {
                    //TODO: create JWT token upon succesfull authentication
                    //var token = _repository.GenerateJwtToken(user);
                    return Ok();
                }
                else
                {
                    return StatusCode(401, "Incorrect credentials");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
