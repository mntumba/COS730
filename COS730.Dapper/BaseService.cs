using COS730.DBContext.Dapper;
using Microsoft.Extensions.Logging;

namespace COS730.Dapper
{
    public class BaseService
    {
        private readonly DapperConnection _dbConnection;
        private bool disposed = false;
        protected readonly ILogger Logger;

        public ApiDBContext DBContext { get { return _dbConnection.DBContext; } }

        public BaseService(DapperConnection connection, ILogger logger)
        {
            _dbConnection = connection;
            this.Logger = logger;
        }

        ~BaseService()
        {
            if (disposed)
                return;

            disposed = true;
        }
    }
}
