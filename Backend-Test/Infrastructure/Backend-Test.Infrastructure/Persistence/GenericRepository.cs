using Backend_Test.Domain.Base;
using Backend_Test.Domain.Interfaces;
using Dapper;
using DapperExtensions;
using DapperExtensions.Predicate;
using DapperExtensions.Sql;
using System.Data;
using System.Linq.Expressions;
using System.Text;
namespace Backend_Test.Infrastructure.Persistence
{


    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public GenericRepository(IDbConnection connection, IDbTransaction transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task InsertAsync(T entity)
            => await _connection.InsertAsync(entity);

        public async Task UpdateAsync(T entity)
            => await _connection.UpdateAsync(entity);

        public async Task DeleteAsync(T entity)
            => await _connection.DeleteAsync(entity);
        public async Task<T> FirstOrDefaultAsync(IPredicate predicate, IList<ISort> sort = null)
        {
            var result = await _connection.GetListAsync<T>(predicate, sort);

            return result.FirstOrDefault();
        }
        public async Task<T> GetByIdAsync(object id)
            => await _connection.GetAsync<T>(id);

        public async Task<long> CountAsync(IPredicate predicate = null)
            => await _connection.CountAsync<T>(predicate);

        public async Task<IEnumerable<T>> GetAllAsync(
            IPredicate predicate = null,
            IList<ISort> sort = null,
            int? skip = null,
            int? take = null)
        {
            if (skip.HasValue && take.HasValue)
            {
                return await _connection.GetPageAsync<T>(
                    predicate,
                    sort,
                    skip.Value / take.Value + 1, // DapperExtensions pages are 1-based
                    take.Value);
            }

            return await _connection.GetListAsync<T>(predicate, sort);
        }
    }


}
