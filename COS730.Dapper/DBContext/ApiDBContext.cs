using COS730.Models.DBModels;
using Microsoft.EntityFrameworkCore;

namespace COS730.DBContext.Dapper
{
    public class ApiDBContext : DbContext
    {
        public ApiDBContext(DbContextOptions<ApiDBContext> options)
            : base(options)
        {

        }

        public DbSet<User>? User { get; set; }
        public DbSet<Message>? Message { get; set; }
    }
}
