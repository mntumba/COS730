using COS730.Models.DBModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COS730.DBContext.Dapper
{
    public class ApiDBContext : DbContext
    {
        public ApiDBContext(DbContextOptions<ApiDBContext> options)
            : base(options)
        {

        }

        public DbSet<User>? User { get; set; }
    }
}
