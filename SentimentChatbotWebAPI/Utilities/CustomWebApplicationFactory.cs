using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using SentimentChatbotWebAPI.Data;
using SentimentChatbotWebAPI.Interfaces;
using SentimentChatbotWebAPI.Repository;

namespace SentimentChatbotWebAPI.Utilities
{
    public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var args = Array.Empty<string>();

            var builder = WebApplication.CreateBuilder(args);

            // Mimic Program.cs depdencies, configuration, services, and middleware
            builder.Services.AddControllers();
            builder.Services.AddAuthorization();

            var mockAzureKeyVaultService = new Mock<IAzureKeyVaultWrapper>();
            mockAzureKeyVaultService.Setup(s => s.GetSecret(It.IsAny<string>())).Returns("MockSecretValue");
            builder.Services.AddSingleton(mockAzureKeyVaultService.Object);

            builder.Services.AddSingleton<IChatbotRepository, ChatbotRepository>();

            // Use in-memory database for testing
            var contextOptions = new DbContextOptionsBuilder<SentimentQueryHistoryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;


            // Then return the builder object
            return builder.Host;
        }
    }

}
