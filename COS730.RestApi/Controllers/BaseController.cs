using COS730.Dapper;
using COS730.DBContext.Dapper;
using Microsoft.AspNetCore.Mvc;

namespace COS730.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected ILogger Logger { get; }
        private readonly ApiDBContext _dbContext;
        public BaseController(ILogger logger)
        {
            this.Logger = logger;

            var apiDbContextFactory = new ApiDBContextFactory();
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
