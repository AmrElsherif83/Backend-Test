using Backend_Test.Infrastructure.Persistence;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Tests.IntegrationTests
{
    public class DatabaseFixture : IDisposable
    {
        public IDbConnection Connection { get; private set; }

        public DatabaseFixture()
        {
            // ✅ Load test configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // ✅ Load test settings
                .Build();

            var dbPath = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(dbPath))
            {
                throw new InvalidOperationException("Database connection string is missing.");
            }

            // ✅ Ensure the directory and database file exist
            var databaseDirectory = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(databaseDirectory))
            {
                Directory.CreateDirectory(databaseDirectory);
            }

            if (!File.Exists(dbPath))
            {
                using (var conn = new SqliteConnection($"Data Source={dbPath};"))
                {
                    conn.Open();
                    var sql = @"
                    CREATE TABLE IF NOT EXISTS Drivers (
                        Id TEXT PRIMARY KEY,
                        FirstName TEXT NOT NULL,
                        LastName TEXT NOT NULL UNIQUE,
                        Email TEXT NOT NULL UNIQUE,
                        PhoneNumber TEXT NOT NULL
                    );";
                    conn.Execute(sql);
                }
            }

            // ✅ Open a persistent SQLite connection for tests
            Connection = new SqliteConnection($"Data Source={dbPath};");
            Connection.Open();
        }

        public void Dispose()
        {
            Connection?.Close();
            Connection?.Dispose();
        }
    }
}
