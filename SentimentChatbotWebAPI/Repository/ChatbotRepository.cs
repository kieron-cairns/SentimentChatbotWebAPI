using Microsoft.IdentityModel.Tokens;
using SentimentChatbotWebAPI.Interfaces;
using SentimentChatbotWebAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_azureSecretClientWrapper.GetSecret("JWT-Secret-Token")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Username),
                new Claim(ClaimTypes.Role,user.Role)
            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
            claims,
                expires: DateTime.UtcNow.AddSeconds(30),
                signingCredentials: credentials);

            var tokenString = _jwtTokenHandler.WriteToken(token);


            // Configure token validation parameters
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true // Enable validation of the token's lifetime
            };

            // Validate the token
            var principal = _jwtTokenHandler.ValidateToken(tokenString, validationParameters, out var validatedToken);

            return tokenString;
        }

    }
}
