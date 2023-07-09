using Microsoft.EntityFrameworkCore;
using SentimentChatbotWebAPI.Models;

namespace SentimentChatbotWebAPI.Interfaces
{
    public interface ISentimentQueryHistoryContext
    {
        DbSet<QueryHistory> QueryHistories { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}