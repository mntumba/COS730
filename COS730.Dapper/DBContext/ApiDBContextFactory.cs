using COS730.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace COS730.DBContext.Dapper
{
    public class ApiDBContextFactory : IDesignTimeDbContextFactory<ApiDBContext>
    {
        private readonly SQLConnectionSettings _sQLConnectionSettings;
        public ApiDBContextFactory(IOptions<SQLConnectionSettings> sQLConnectionSettings)
        {
            _sQLConnectionSettings = sQLConnectionSettings.Value;
        }

        public ApiDBContext CreateDbContext(string[] args)
        {
            var dbContextBuilder = new DbContextOptionsBuilder<ApiDBContext>();
            dbContextBuilder.UseSqlServer(_sQLConnectionSettings.ConnectionString);
            return new ApiDBContext(dbContextBuilder.Options);
        }
    }
}
