using Backend_Test.Domain.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection GetConnection();
        IGenericRepository<T> Repository<T>() where T : BaseEntity;
        void BeginTransaction();
        Task CommitAsync();
        void Rollback();
        bool IsTransactionActive { get; }
        IDbTransaction GetTransaction();
    }

}
