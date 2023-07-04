using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace SentimentChatbotWebAPI.Interfaces
{
    public interface IJwtTokenHandler
    {
        string WriteToken(SecurityToken token);
        ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken);
    }
}