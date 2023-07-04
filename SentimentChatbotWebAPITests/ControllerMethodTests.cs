using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SentimentChatbotWebAPI.Controllers;
using SentimentChatbotWebAPI.Interfaces;
using SentimentChatbotWebAPI.Models;
using SentimentChatbotWebAPITests.MockObjects;
using System.Net;

namespace SentimentChatbotWebAPITests
{
    public class ControllerMethodTests
    {

        private readonly Mock<IAzureSecretClientWrapper> _azureSecretClientWrapper;
        private Mock<IConfiguration> _configurationMock;

        public ControllerMethodTests()
        {
            _azureSecretClientWrapper = new Mock<IAzureSecretClientWrapper>();
        }

        [Fact]
        public async Task AuthenticateUser_Retuns_Unauthorized()
        {
            var httpClientMock = new Mock<IHttpClientFactory>();
            var httpClient = new HttpClient(new MockHttpMessageHandler((request, cancellationToken) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);

                return Task.FromResult(response);
            }));

            var httpContextMock = new Mock<HttpContext>();

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.SetupGet(a => a.HttpContext).Returns(httpContextMock.Object);

            var chatbotRepositoryMock = new Mock<IChatbotRepository>();

            var configurationMock = new Mock<IConfiguration>();

            var controller = new ChatbotController(httpClientMock.Object, httpContextAccessorMock.Object, chatbotRepositoryMock.Object, _azureSecretClientWrapper.Object, configurationMock.Object);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.HttpContext.Request.Headers["username"] = "invalidusername";
            controller.HttpContext.Request.Headers["password"] = "invalidpassword";

            //Act

            var result = controller.AuthenticateUser();

            //Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(401, objectResult.StatusCode);
        }


        [Fact]
        public async Task AuthenticateUser_Returns_OK()
        {
            // Arrange
            var httpClientMock = new Mock<IHttpClientFactory>();
            var httpClient = new HttpClient(new MockHttpMessageHandler((request, cancellationToken) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                return Task.FromResult(response);
            }));

            var httpContextMock = new Mock<HttpContext>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.SetupGet(a => a.HttpContext).Returns(httpContextMock.Object);

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["TestUsers:Username"]).Returns("validusername");
            configurationMock.Setup(x => x["TestUsers:Password"]).Returns("validpassword");

            var secretClientWrapperMock = new Mock<IAzureSecretClientWrapper>();
            secretClientWrapperMock.Setup(r => r.GetSecret("validusername")).Returns("validusername");
            secretClientWrapperMock.Setup(r => r.GetSecret("validpassword")).Returns("validpassword");

            var chatbotRepositoryMock = new Mock<IChatbotRepository>();
            chatbotRepositoryMock.Setup(r => r.GenerateJwtToken(It.IsAny<User>())).Returns("testToken");

            var controller = new ChatbotController(
                httpClientMock.Object,
                httpContextAccessorMock.Object,
                chatbotRepositoryMock.Object,
                secretClientWrapperMock.Object,
                configurationMock.Object
            );

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.HttpContext.Request.Headers["username"] = "validusername";
            controller.HttpContext.Request.Headers["password"] = "validpassword";

            // Act
            var result = controller.AuthenticateUser();

            // Assert
            var objectResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }
    }
}