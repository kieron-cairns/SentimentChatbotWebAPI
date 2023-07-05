using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using SentimentChatbotWebAPI.Data;
using SentimentChatbotWebAPI.Interfaces;
using SentimentChatbotWebAPI.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SentimentChatbotWebAPI.Repository;

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

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IChatbotRepository, ChatbotRepository>();
builder.Services.AddScoped<ISentimentQueryHistoryContext, SentimentQueryHistoryContext>();
builder.Services.AddScoped<IJwtTokenHandler, JwtTokenHandlerWrapper>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IAzureKeyVaultWrapper>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    string keyVaultUrl = config["keyVaultConfig:KVUrl"];
    string tenantId = config["keyVaultConfig:TenantId"];
    string clientId = config["keyVaultConfig:ClientId"];
    string clientSecretId = config["keyVaultConfig:ClientSecretId"];

    var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecretId);
    var secretClient = new SecretClient(new Uri(keyVaultUrl), clientSecretCredential);
    return new AzureKeyVaultWrapper(secretClient);
});

builder.Services.AddSingleton<IAzureSecretClientWrapper, AzureSecretClientWrapper>();

var jwtSecretToken = client.GetSecret(builder.Configuration.GetSection("JWTConfig:TokenName").Value).Value.Value;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretToken))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
