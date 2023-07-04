using Castle.Core.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SentimentChatbotWebAPI.Interfaces;
using SentimentChatbotWebAPI.Models;
using SentimentChatbotWebAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }
    }

    
}
