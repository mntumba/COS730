using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COS730.Models.DBModels
{
    [Table("User")]
    public class User
    {
        [Key]
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? PreferedLanguage { get; set; }
        public string? OTP { get; set; }
        public bool IsVerified { get; set; }

    }
}
