using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SentimentChatbotWebAPI.Interfaces;
using SentimentChatbotWebAPI.Models;
using System.Net;
using System.Text;
using System.Text.Json;

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
                    return Ok(token);
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

        [HttpGet("/VerifyBearer")]
        [Authorize]
        public IActionResult VerifyBearer()
        {
            return Ok();
        }

        [HttpPost("/AnalyseQuery")]
        [Authorize]
        public async Task<IActionResult> AnalyzeSentiment([FromBody] dynamic jsonData)
        {
            try
            {
                var url = "http://localhost:7055/api/AnalyzeSentiment";

                var jsonBody = jsonData.ToString();
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                // TODO: Process the response if needed

                var sentimentResult = new SentimentResult
                {
                    Result = responseContent
                };

                // Return the JsonResult immediately
                var jsonResult = new JsonResult(sentimentResult);

                return jsonResult;
            }
            catch (Exception ex)
            {
                // Handle exception appropriately 
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPost("/PostToSql")]
        [Authorize]
        public async Task<IActionResult> PostQueryToSql([FromBody] dynamic jsonData)
        {
            var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            try
            {
                var url = "http://localhost:7055/api/AnalyzeSentiment";
                var jsonBody = jsonData.ToString();
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                JsonDocument jsonQueryText = JsonDocument.Parse(jsonBody);
                JsonElement rootElement = jsonQueryText.RootElement;
                string queryText = rootElement.GetProperty("SentimentText").GetString();

                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                var sentimentResult = new SentimentResult
                {
                    Result = responseContent
                };

                var jsonResult = new JsonResult(sentimentResult);
                jsonResult.StatusCode = (int)HttpStatusCode.OK;

                _repository.WriteQueryToSql(ipAddress, queryText, sentimentResult.Result);

                return jsonResult;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("/GetQueriesByIp")]
        [Authorize]
        public IActionResult GetAllItemsByIp()
        {
            var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            List<QueryHistory> items = _repository.GetAllItemsByIp(ipAddress);
            return Ok(items);
        }

        [HttpDelete("/DeleteAllByIp")]
        [Authorize]
        public async Task<IActionResult> DeleteAllItemsByIpAddress()
        {
            var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            try
            {
                _repository.DeleteAllByIpAddress(ipAddress);
                //_httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                return Ok();

            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
    }
}
