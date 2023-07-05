using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SentimentChatbotWebAPI.Data;
using SentimentChatbotWebAPI.Interfaces;
using SentimentChatbotWebAPI.Models;
using SentimentChatbotWebAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SentimentChatbotWebAPITests
{
    public class RepsitoryMethodTests
    {
        private readonly Mock<IJwtTokenHandler> _jwtTokenHandlerMock;
        private readonly Mock<IAzureSecretClientWrapper> _azureSecretClientWrapperMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly User _user;
        private readonly ChatbotRepository _chatbotRepository;
        private SecurityToken _fakeToken;


        public RepsitoryMethodTests()
        {
            _jwtTokenHandlerMock = new Mock<IJwtTokenHandler>();
            _azureSecretClientWrapperMock = new Mock<IAzureSecretClientWrapper>();
            _configurationMock = new Mock<IConfiguration>();
            _user = new User { Username = "testUsername", Password = "TestPassword", Role = "testRole" };

        }

        [Fact]
        public void Generate_JWT_Token_Succesfully_Creates_Token()
        {
            var mockContext = new Mock<ISentimentQueryHistoryContext>();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKey"));
            _jwtTokenHandlerMock.Setup(th => th.WriteToken(It.IsAny<SecurityToken>())).Returns("fakeTokenString");
            _jwtTokenHandlerMock.Setup(th => th.ValidateToken(It.IsAny<string>(), It.IsAny<TokenValidationParameters>(), out _fakeToken))
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new[] {
        new Claim(ClaimTypes.NameIdentifier, _user.Username),
        new Claim(ClaimTypes.Role, _user.Role)
                })));

            _azureSecretClientWrapperMock.Setup(sPM => sPM.GetSecret("JWT-Secret-Token")).Returns("YourSecretKey");

            var repository = new ChatbotRepository(mockContext.Object, _configurationMock.Object, _jwtTokenHandlerMock.Object, _azureSecretClientWrapperMock.Object);

            var tokenString = repository.GenerateJwtToken(_user);

            // Assert that tokenString is correct
            Assert.Equal("fakeTokenString", tokenString);

            // Now you can make further assertions based on what your method is supposed to do.
        }

        [Fact]
        public void Generate_JWT_Token_Unsuccessfully_When_Invalid_Secret()
        {
            var mockContext = new Mock<ISentimentQueryHistoryContext>();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKey"));
            _jwtTokenHandlerMock.Setup(th => th.WriteToken(It.IsAny<SecurityToken>())).Returns("fakeTokenString");

            // Setup ValidateToken to throw an exception
            _jwtTokenHandlerMock.Setup(th => th.ValidateToken(It.IsAny<string>(), It.IsAny<TokenValidationParameters>(), out _fakeToken))
                .Throws(new SecurityTokenInvalidSignatureException());

            _azureSecretClientWrapperMock.Setup(sPM => sPM.GetSecret("JWT-Secret-Token")).Returns("YourSecretKey");

            var repository = new ChatbotRepository(mockContext.Object, _configurationMock.Object, _jwtTokenHandlerMock.Object, _azureSecretClientWrapperMock.Object);

            var tokenString = repository.GenerateJwtToken(_user);

            // Assert that tokenString is null or whatever you decided to return in the case of an exception
            Assert.Null(tokenString);
        }

    }


}
