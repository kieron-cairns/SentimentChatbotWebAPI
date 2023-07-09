using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using SentimentChatbotWebAPI.Controllers;
using SentimentChatbotWebAPI.Interfaces;
using SentimentChatbotWebAPI.Models;
using SentimentChatbotWebAPITests.MockObjects;
using System.Net;
using Newtonsoft.Json;
using System.Text;

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
            var objectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public async Task AnalyzeSentiment_ReturnsJsonResult_WhenCalledWithValidData()
        {
            // Arrange
            var expectedUri = new Uri("http://localhost:7055/api/AnalyzeSentiment");

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri == expectedUri),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject("Positive")),
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var httpContextMock = new Mock<HttpContext>();

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.SetupGet(a => a.HttpContext).Returns(httpContextMock.Object);

            var chatbotRepositoryMock = new Mock<IChatbotRepository>();

            var configurationMock = new Mock<IConfiguration>();

            var secretClientWrapperMock = new Mock<IAzureSecretClientWrapper>();
            secretClientWrapperMock.Setup(r => r.GetSecret("validusername")).Returns("validusername");
            secretClientWrapperMock.Setup(r => r.GetSecret("validpassword")).Returns("validpassword");

            var controller = new ChatbotController(
                httpClientFactoryMock.Object,
                httpContextAccessorMock.Object,
                chatbotRepositoryMock.Object,
                secretClientWrapperMock.Object,
                configurationMock.Object
            );

            // Act
            dynamic data = new { text = "{\"SentimentText\" : \"Today is a very good day\"}" };
            var result = await controller.AnalyzeSentiment(data);

            // Assert
            var actionResult = Assert.IsType<JsonResult>(result);
            var model = Assert.IsType<SentimentResult>(actionResult.Value);
            //Assert.Equal("Expected Result", model.Result);
            Assert.Equal(JsonConvert.SerializeObject("Positive"), model.Result);

        }

        [Fact]
        public async Task PostToSql_Returns_SuccesfullJsonResult()
        {
            //Arrange
            var jsonDataString = new InputJsonBoody
            {
                SentimentText = "The weather is fantastic today!"
            };

            //Serialize the json input
            string jsonData = System.Text.Json.JsonSerializer.Serialize(jsonDataString);

            var ipAddress = "127.0.0.1";
            var httpClientMock = new Mock<IHttpClientFactory>();
            var httpClient = new HttpClient(new MockHttpMessageHandler((request, cancellationToken) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"result\": \"Positive\"}", Encoding.UTF8, "application/json")
                };
                return Task.FromResult(response);
            }));

            var configurationMock = new Mock<IConfiguration>();


            httpClientMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var sentimentRepositoryMock = new Mock<IChatbotRepository>();
            sentimentRepositoryMock.Setup(_ => _.WriteQueryToSql(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var httpContextMock = new Mock<HttpContext>();

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.SetupGet(a => a.HttpContext).Returns(httpContextMock.Object);
            httpContextAccessorMock.SetupGet(c => c.HttpContext.Connection.RemoteIpAddress).Returns(IPAddress.Parse(ipAddress));

            var controller = new ChatbotController(httpClientMock.Object, httpContextAccessorMock.Object, sentimentRepositoryMock.Object, _azureSecretClientWrapper.Object, configurationMock.Object);

            // Create a mock ConnectionInfo with a mock RemoteIpAddress
            var connectionInfoMock = new Mock<ConnectionInfo>();
            connectionInfoMock.SetupGet(c => c.RemoteIpAddress).Returns(IPAddress.Parse(ipAddress));

            // Create a mock HttpContext and set the Connection property

            httpContextMock.SetupGet(c => c.Connection).Returns(connectionInfoMock.Object);

            // Assign the mock HttpContext to the controller's ControllerContext
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            //Act
            var result = await controller.PostQueryToSql(jsonData);

            //Assert
            Assert.IsType<JsonResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var sentimentResult = Assert.IsType<SentimentResult>(jsonResult.Value);
            Assert.Equal("{\"result\": \"Positive\"}", sentimentResult.Result);
            Assert.Equal(200, jsonResult.StatusCode);

            sentimentRepositoryMock.Verify(_ => _.WriteQueryToSql(ipAddress, It.IsAny<string>(), sentimentResult.Result), Times.Once);
        }

    }
}