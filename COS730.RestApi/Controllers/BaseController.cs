using COS730.Dapper;
using COS730.DBContext.Dapper;
using COS730.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace COS730.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected ILogger Logger { get; }
        private readonly ApiDBContext _dbContext;
        public BaseController(ILogger logger, IOptions<SQLConnectionSettings> sQLConnectionSettings)
        {
            this.Logger = logger;

            var apiDbContextFactory = new ApiDBContextFactory(sQLConnectionSettings);
            _dbContext = apiDbContextFactory.CreateDbContext(Array.Empty<string>());
        }

        public ApiDBContext DBContext
        {
            get
            {
                return _dbContext;
            }
        }

        public DapperConnection DBConnection
        {
            get
            {
                var dbConnection = new DapperConnection(_dbContext);

                return dbConnection;
            }
        }
    }
}
