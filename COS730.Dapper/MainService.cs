using COS730.DBContext.Dapper;
using Microsoft.Extensions.Logging;

namespace COS730.Dapper
{
    public class MainService
    {
        private readonly DapperConnection _dbConnection;
        private bool disposed = false;
        protected readonly ILogger Logger;

        public ApiDBContext DBContext { get { return _dbConnection.DBContext; } }

        public MainService(DapperConnection connection, ILogger logger)
        {
            _dbConnection = connection;
            this.Logger = logger;
        }

        ~MainService()
        {
            if (disposed)
                return;

            disposed = true;
        }
    }
}
