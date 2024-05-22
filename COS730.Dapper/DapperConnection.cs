using Microsoft.EntityFrameworkCore.Storage;
using COS730.DBContext.Dapper;
using Microsoft.EntityFrameworkCore;

namespace COS730.Dapper
{
    public class DapperConnection
    {
        private IDbContextTransaction? _transaction;
        bool disposed = false;

        private readonly ApiDBContext _dbContext;

        public DapperConnection(ApiDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ApiDBContext DBContext
        {
            get { return _dbContext; }
        }

        public void BeginTransaction()
        {
            if (_transaction != null)
                return;

            if (_dbContext == null && !_dbContext!.Database.CanConnect())
                throw new Exception("Cannot connect to DB.");

            _transaction = _dbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
                return;

            if (_dbContext == null && !_dbContext!.Database.CanConnect())
                throw new Exception("Cannot connect to DB.");

            _dbContext.Database.CommitTransaction();
            _transaction = null;
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
                return;

            if (_dbContext == null && !_dbContext!.Database.CanConnect())
                throw new Exception("Cannot connect to DB.");

            _dbContext.Database.RollbackTransaction();
            _transaction = null;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        private bool disposedValue = false;

        public void Dispose()
        {
            if (disposed)
                return;

            if (true)
            {
                if (!disposedValue)
                {
                    if (true)
                    {
                        if (_dbContext == null)
                            throw new Exception("Connection is NULL");

                        if (_transaction != null)
                        {
                            _transaction.Rollback();
                            _transaction = null;
                        }

                        if (_dbContext.Database.CanConnect())
                            _dbContext.Database.CloseConnection();
                        _dbContext.Dispose();
                    }
                    disposedValue = true;
                }
            }

            disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
