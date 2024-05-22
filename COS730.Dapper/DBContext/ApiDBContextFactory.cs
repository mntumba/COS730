using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace COS730.DBContext.Dapper
{
    public class ApiDBContextFactory : IDesignTimeDbContextFactory<ApiDBContext>
    {
        public ApiDBContext CreateDbContext(string[] args)
        {
            var dbContextBuilder = new DbContextOptionsBuilder<ApiDBContext>();
            dbContextBuilder.UseSqlServer("Server=.;Database=COS-730;Trusted_Connection=True;TrustServerCertificate=True;");
            return new ApiDBContext(dbContextBuilder.Options);
        }
    }
}
