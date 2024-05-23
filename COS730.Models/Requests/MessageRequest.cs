
namespace COS730.Models.Requests
{
    public class MessageRequest
    {
        public string? SenderEmail { get; set; }
        public string? RecipientEmail { get; set; }
        public string? Message { get; set; }
    }
}
