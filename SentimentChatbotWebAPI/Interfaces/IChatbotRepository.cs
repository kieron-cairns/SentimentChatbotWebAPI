using SentimentChatbotWebAPI.Models;

namespace SentimentChatbotWebAPI.Interfaces
{
    public interface IChatbotRepository
    {
        string GenerateJwtToken(User user);
        Task WriteQueryToSql(string ipAddress, string queryText, string queryResult);
        List<QueryHistory> GetAllItemsByIp(string ipAddress);
        Task DeleteAllByIpAddress(string ipAddress);
    }
}