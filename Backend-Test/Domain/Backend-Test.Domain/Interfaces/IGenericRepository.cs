using Backend_Test.Domain.Base;
using DapperExtensions;
using DapperExtensions.Predicate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task InsertAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T> GetByIdAsync(object id);
        Task<T> FirstOrDefaultAsync(IPredicate predicate, IList<ISort> sort = null);

        Task<IEnumerable<T>> GetAllAsync(
            IPredicate predicate = null,
            IList<ISort> sort = null,
            int? skip = null,
            int? take = null);
        Task<long> CountAsync(IPredicate predicate = null);
    }



}
