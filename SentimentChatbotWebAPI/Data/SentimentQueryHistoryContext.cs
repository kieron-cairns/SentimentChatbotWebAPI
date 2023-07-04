using Microsoft.EntityFrameworkCore;
using SentimentChatbotWebAPI.Interfaces;
using SentimentChatbotWebAPI.Models;

namespace SentimentChatbotWebAPI.Data
{
    public class SentimentQueryHistoryContext : DbContext, ISentimentQueryHistoryContext
    {
        public DbSet<QueryHistory> QueryHistories { get; set; } = null!;
        public SentimentQueryHistoryContext(DbContextOptions<SentimentQueryHistoryContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
