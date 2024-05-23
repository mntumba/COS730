using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COS730.Models.DBModels
{
    [Table("Message")]
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? SenderEmail { get; set; }
        public string? RecipientEmail { get; set; }
        public byte[]? MessageData { get; set; }
        public byte[]? MessageKey { get; set; }
        public byte[]? MessageIV { get; set; }
    }
}
