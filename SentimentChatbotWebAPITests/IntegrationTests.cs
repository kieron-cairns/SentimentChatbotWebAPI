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

namespace SentimentChatbotWebAPITests
{

    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>> // replace 'YourNamespace' with the actual namespace of your Program class
    {
        private readonly WebApplicationFactory<Program> _factory; // replace 'YourNamespace' with the actual namespace of your Program class

        public IntegrationTests(WebApplicationFactory<Program> factory) // replace 'YourNamespace' with the actual namespace of your Program class
        {
            _factory = factory;
        }

        [Fact]
        public async Task VerifyBearer_AuthorizedUser_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Simulate Authentication by setting up Bearer token
            // This should be replaced by a function that generates a valid token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your valid JWT token here");

            // Act
            var response = await client.GetAsync("/VerifyBearer");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }


}
