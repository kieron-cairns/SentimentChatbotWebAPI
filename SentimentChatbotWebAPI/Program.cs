using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using SentimentChatbotWebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Retrieve the SQL connection string from Azure Key Vault
var keyVaultUrl = new Uri(builder.Configuration.GetSection("keyVaultConfig:KVUrl").Value!);
var keyVaultTenantId = builder.Configuration.GetSection("keyVaultConfig:TenantId").Value;
var keyVaultClientId = builder.Configuration.GetSection("keyVaultConfig:ClientId").Value;
var keyVaultSecret = builder.Configuration.GetSection("keyVaultConfig:ClientSecretId").Value;

var azureCredential = new ClientSecretCredential(keyVaultTenantId, keyVaultClientId, keyVaultSecret);

var client = new SecretClient(keyVaultUrl, azureCredential);

var connectionString = client.GetSecret(builder.Configuration.GetSection("ConnectionStrings:Test-DB").Value).Value.Value;

// Add the DbContext with the SQL connection string to the service container
builder.Services.AddDbContext<SentimentQueryHistoryContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
