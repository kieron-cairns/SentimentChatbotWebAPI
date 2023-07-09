using Microsoft.EntityFrameworkCore;
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

        private static DbSet<T> MockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet.Object;
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

        [Fact]
        public async void Write_Query_To_SQL_Successfuly_Executes()
        {
            //Arrange
            var mockContext = new Mock<ISentimentQueryHistoryContext>();
            var mockSet = new Mock<DbSet<QueryHistory>>();
            //var mockITokenHandler = new Mock<ITokenHandler>();

            mockContext.Setup(x => x.QueryHistories).Returns(mockSet.Object);

            var repository = new ChatbotRepository(mockContext.Object, _configurationMock.Object, _jwtTokenHandlerMock.Object, _azureSecretClientWrapperMock.Object);

            string queryText = "{'SentimentText' : 'Today is a good day'}";
            string queryResult = "Positive";
            string ipAddress = "192.168.1.0";

            //Act
            await repository.WriteQueryToSql(ipAddress, queryText, queryResult);

            //Assert
            mockSet.Verify(s => s.Add(It.IsAny<QueryHistory>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void GetAllItemsByIp_Returns_Items_With_Correct_IpAddress()
        {
            // Arrange
            var ipAddress = "127.0.0.1";
            var expectedItems = new List<QueryHistory>()
            {
                new QueryHistory { Id = Guid.NewGuid(), IpAddress = "127.0.0.1", Date = DateTime.Now, QueryText = "The weather is good today", QueryResult = "Positive" },
                new QueryHistory { Id = Guid.NewGuid(), IpAddress = "127.0.0.1", Date = DateTime.Now, QueryText = "The weather is bad today", QueryResult = "Negative" },
            };

            var queryHistories = new List<QueryHistory>()
            {
            new QueryHistory { Id = Guid.NewGuid(), IpAddress = "127.0.0.1", Date = DateTime.Now, QueryText = "The weather is good today", QueryResult = "Positive" },
            new QueryHistory { Id = Guid.NewGuid(), IpAddress = "127.0.0.1", Date = DateTime.Now, QueryText = "The weather is bad today", QueryResult = "Negative" },
            new QueryHistory { Id = Guid.NewGuid(), IpAddress = "127.0.1.1", Date = DateTime.Now, QueryText = "The film I watched at the cinema was amazing!", QueryResult = "Positive" },
            }.AsQueryable();

            var dbContextMock = new Mock<ISentimentQueryHistoryContext>();
            dbContextMock.Setup(x => x.QueryHistories).Returns(MockDbSet(queryHistories));

            var repository = new ChatbotRepository(dbContextMock.Object, _configurationMock.Object, _jwtTokenHandlerMock.Object, _azureSecretClientWrapperMock.Object);

            // Act
            var result = repository.GetAllItemsByIp(ipAddress);

            // Assert
            Assert.Equal(expectedItems.Count, result.Count);
            Assert.All(result, item => Assert.Equal(ipAddress, item.IpAddress));
            // Perform additional assertions on the retrieved items as needed
        }

    }




}
