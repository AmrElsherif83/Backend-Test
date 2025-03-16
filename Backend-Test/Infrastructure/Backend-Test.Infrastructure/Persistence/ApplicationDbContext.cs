using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Infrastructure.Persistence
{
    public class ApplicationDbContext
    {
        private readonly IDbConnection _connection;

        public ApplicationDbContext(string connectionString)
        {
            _connection = new SqliteConnection(connectionString);
        }

        public IDbConnection Connection => _connection;
    }

}
