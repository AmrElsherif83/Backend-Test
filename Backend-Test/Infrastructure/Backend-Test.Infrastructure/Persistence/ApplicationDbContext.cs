using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Infrastructure.Persistence
{
    public class ApplicationDbContext : IDisposable
    {
        public IDbConnection Connection { get; }

        public ApplicationDbContext(string connectionString)
        {
            Connection = new SqliteConnection(connectionString);
            Connection.Open();
        }

      
        public void Dispose() => Connection.Dispose();
    }

}
