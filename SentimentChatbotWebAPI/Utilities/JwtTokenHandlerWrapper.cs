using Microsoft.IdentityModel.Tokens;
using SentimentChatbotWebAPI.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SentimentChatbotWebAPI.Utilities
{
    public class JwtTokenHandlerWrapper : IJwtTokenHandler
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtTokenHandlerWrapper()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public string WriteToken(SecurityToken token)
        {
            return _tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            return _tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
        }
    }
}
