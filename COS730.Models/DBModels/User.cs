using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COS730.Models.DBModels
{
    [Table("User")]
    public class User
    {
        [Key]
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? OTP { get; set; }
        public bool IsVerified { get; set; }

    }
}
