using Backend_Test.Domain.Base;
using Backend_Test.Domain.Interfaces;
using System.Data;
using Microsoft.Data.Sqlite;
using DapperExtensions;
using Microsoft.Extensions.Logging;

namespace Backend_Test.Infrastructure.Persistence
{
   

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;
        private bool _disposed;
        private readonly ILogger<UnitOfWork> _logger;
        private readonly Dictionary<Type, object> _repositories = new();
        public bool IsTransactionActive { get; private set; }
        public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        {
            _connection = context.Connection;
            _logger = logger;
        }

        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            if (_repositories.ContainsKey(typeof(T)))
                return (IGenericRepository<T>)_repositories[typeof(T)];

            var repository = new GenericRepository<T>(_connection, _transaction);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        public void BeginTransaction()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open(); // ✅ Open connection if not already open
            }
            _transaction ??= _connection.BeginTransaction();
            _logger.LogInformation("Transaction started");
            IsTransactionActive = true;
        }

        public async Task CommitAsync()
        {
            try
            {
                _transaction?.Commit();
                _logger.LogInformation("Transaction committed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction commit failed");
                _transaction?.Rollback();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
                IsTransactionActive = false;
            }

            await Task.CompletedTask;
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _logger.LogWarning("Transaction rolled back");
            _transaction?.Dispose();
            _transaction = null;
            IsTransactionActive = false;
        }
        public IDbTransaction GetTransaction()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction has not been started. Call BeginTransaction first.");

            return _transaction;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _transaction?.Dispose();
            _connection?.Dispose();
            _disposed = true;
        }
        public IDbConnection GetConnection()
        {
            return _connection;
        }
    }

}
