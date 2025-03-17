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
        public async Task<bool> ExistsAsync(object id)
        {
            var tableName = typeof(T).Name;
            var sql = $"SELECT COUNT(1) FROM {tableName} WHERE Id = @Id";

            var count = await _connection.ExecuteScalarAsync<int>(sql, new { Id = id });
            return count > 0;
        }
        public async Task InsertAsync(T entity)
            => await _connection.InsertAsync(entity);

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var properties = typeof(T).GetProperties();
            var tableName = typeof(T).Name;

            var updateFields = string.Join(", ", properties
                .Where(p => p.Name != "Id") // ✅ Exclude Id from update statement
                .Select(p => $"{p.Name} = @{p.Name}"));

            var sql = $"UPDATE {tableName} SET {updateFields} WHERE Id = @Id";

            var parameters = new DynamicParameters();
            foreach (var property in properties)
            {
                parameters.Add($"@{property.Name}", property.GetValue(entity));
            }
            var success = await _connection.ExecuteAsync(sql, parameters);
            if (success == 0)
                throw new InvalidOperationException("Update failed. Ensure all required parameters are set.");

        }

        public async Task DeleteAsync(T entity)
            => await _connection.DeleteAsync(entity);
        public async Task<T> FirstOrDefaultAsync(IPredicate predicate, IList<ISort> sort = null)
        {
            var result = await _connection.GetListAsync<T>(predicate, sort);

            return result.FirstOrDefault();
        }
        public async Task<T> GetByIdAsync(Guid id)
        {


            var predicate = Predicates.Field<T>(p => p.Id, Operator.Eq, id); // Ensure proper filtering
            var result = await _connection.GetListAsync<T>(predicate, transaction: _transaction);

            var list = result.ToList();

            if (list.Count == 0)
            {
              
                return null;
            }

            if (list.Count > 1)
            {
              
                throw new InvalidOperationException($"Multiple records found for {typeof(T).Name} with Id={id}");
            }

            return list.SingleOrDefault();
        }

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
