using Microsoft.AspNetCore.Mvc.Testing;

namespace SentimentChatbotWebAPI.Utilities
{
    public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var args = Array.Empty<string>();

            var builder = WebApplication.CreateBuilder(args);

            // Mimic what's in your Program.cs, set up your configuration, services, and middleware
            // This is an example, replace with your actual setup
            builder.Services.AddControllers();
            // Add other necessary services for your application

            // Then return the builder object
            return builder.Host;
        }
    }

}
