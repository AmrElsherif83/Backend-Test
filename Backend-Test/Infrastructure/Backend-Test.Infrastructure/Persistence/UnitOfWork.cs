using Backend_Test.Domain.Base;
using Backend_Test.Domain.Interfaces;
using System.Data;
using Microsoft.Data.Sqlite;
using DapperExtensions;

namespace Backend_Test.Infrastructure.Persistence
{
   

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;
        private bool _disposed;

        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(ApplicationDbContext context)
        {
            _connection = context.Connection;
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
            _transaction ??= _connection.BeginTransaction();
        }

        public async Task CommitAsync()
        {
            try
            {
                _transaction?.Commit();
            }
            catch
            {
                _transaction?.Rollback();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }

            await Task.CompletedTask;
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _transaction?.Dispose();
            _connection?.Dispose();
            _disposed = true;
        }
    }

}
