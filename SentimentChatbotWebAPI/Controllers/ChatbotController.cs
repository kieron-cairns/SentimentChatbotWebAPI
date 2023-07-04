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
            string httpHeaderUsername = Request.Headers["username"].FirstOrDefault();
            string httpHeaderPassword = Request.Headers["password"].FirstOrDefault();

            User user = new User
            {
                Username = httpHeaderUsername,
                Password = httpHeaderPassword,
                Role = "User"
            };
            try
            {
                string username = _configuration["TestUsers:Username"];
                string password = _configuration["TestUsers:Password"];


                if (httpHeaderUsername == _azureSecretClientWrapper.GetSecret(username) && httpHeaderPassword == _azureSecretClientWrapper.GetSecret(password))
                {
                    //TODO: create JWT token upon succesfull authentication
                    var token = _repository.GenerateJwtToken(user);
                    return Ok();
                }
                else
                {
                    return StatusCode(401, "Incorrect credentials");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
