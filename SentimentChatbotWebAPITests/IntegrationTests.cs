using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SentimentChatbotWebAPI.Interfaces;
using Moq;
using SentimentChatbotWebAPI.Models;
using Microsoft.Extensions.Configuration;
using SentimentChatbotWebAPI.Repository;
using Microsoft.Extensions.DependencyInjection;
using SentimentChatbotWebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace SentimentChatbotWebAPITests
{

    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>> // replace 'YourNamespace' with the actual namespace of your Program class
    {
        private readonly WebApplicationFactory<Program> _factory; // replace 'YourNamespace' with the actual namespace of your Program class
        private readonly Mock<IAzureSecretClientWrapper> _azureSecretClientWrapperMock;
        private readonly User _user;
        private readonly SentimentQueryHistoryContext _context;
        private readonly IChatbotRepository _chatbotRepository;

        public IntegrationTests(WebApplicationFactory<Program> factory) // replace 'YourNamespace' with the actual namespace of your Program class
        {
            _factory = factory;
            _azureSecretClientWrapperMock = new Mock<IAzureSecretClientWrapper>();
            _user = new User { Username = "testUsername", Password = "TestPassword", Role = "testRole" };

            //var contextOptions = new DbContextOptionsBuilder<SentimentQueryHistoryContext>()
            //.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            //.Options;

            //_context = new SentimentQueryHistoryContext(contextOptions);
            //_chatbotRepository = new ChatbotRepository(_context, /* other dependencies here */);
        }

        [Fact]
        public async Task VerifyBearer_AuthorizedUser_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            var mockContext = new Mock<ISentimentQueryHistoryContext>();
            _azureSecretClientWrapperMock.Setup(sPM => sPM.GetSecret("JWT-Secret-Token")).Returns("YourSecretKey");

            // Create a scope to retrieve scoped services
            using var scope = _factory.Services.CreateScope();

            // Get an instance of your IChatbotRepository
            var chatbotRepository = scope.ServiceProvider.GetRequiredService<IChatbotRepository>();

            // Simulate Authentication by setting up Bearer token
            var tokenString = chatbotRepository.GenerateJwtToken(_user);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);

            // Act
            var response = await client.GetAsync("/VerifyBearer");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Write_Query_To_SQL_Successfuly_Executes()
        {
            // Arrange
            string queryText = "{'SentimentText' : 'Today is a good day'}";
            string queryResult = "Positive";
            string ipAddress = "192.168.1.0";

            // Create a scope to retrieve scoped services
            using var scope = _factory.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var chatbotRepository = serviceProvider.GetRequiredService<IChatbotRepository>();
            var context = serviceProvider.GetRequiredService<ISentimentQueryHistoryContext>();

            // Act
            await chatbotRepository.WriteQueryToSql(ipAddress, queryText, queryResult);

            // Assert
            var savedQueryHistory = await context.QueryHistories
                .SingleOrDefaultAsync(q => q.QueryText == queryText && q.QueryResult == queryResult && q.IpAddress == ipAddress);

            Assert.NotNull(savedQueryHistory);
        }


        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }

    }
}
