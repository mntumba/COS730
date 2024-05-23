using COS730.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace COS730.DBContext.Dapper
{
    public class ApiDBContextFactory : IDesignTimeDbContextFactory<ApiDBContext>
    {
        public ApiDBContextFactory()
        {
        }

        public ApiDBContext CreateDbContext(string[] args)
        {
            var dbContextBuilder = new DbContextOptionsBuilder<ApiDBContext>();
            dbContextBuilder.UseSqlServer("Server=.;Database=COS-730;Trusted_Connection=True;TrustServerCertificate=True;");
            return new ApiDBContext(dbContextBuilder.Options);
        }
    }
}
