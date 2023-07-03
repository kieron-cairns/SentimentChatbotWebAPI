using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SentimentChatbotWebAPI.Models
{
    public class QueryHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(16)")]
        public string IpAddress { get; set; }

        [Required]
        public DateTime Date { get; set; }
        [Column(TypeName = "nvarchar(300)")]

        [Required]
        public string QueryText { get; set; }
        [Column(TypeName = "nvarchar(8)")]

        [Required]
        public string QueryResult { get; set; }
    }
}
