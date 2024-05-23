
namespace COS730.Models.Responses
{
    public class ChatHistoryResponse
    {
        public int Id { get; set; }
        public string? SenderEmail { get; set; }
        public string? RecipientEmail { get; set; }
        public string? Message { get; set; }
    }
}
