using Backend_Test.Domain.Entities;
using Backend_Test.Domain.Interfaces;
using Backend_Test.Infrastructure.Persistence;
using Backend_Test.Infrastructure.Persistence.Mappings;
using Dapper;
using DapperExtensions;
using DapperExtensions.Sql;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Slapper.AutoMapper;

namespace Backend_Test.Tests.Repositories
{
    public class GenericRepositoryTests : IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<ILogger<UnitOfWork>> _loggerMock;

        public GenericRepositoryTests()
        {
            var connectionString = "DataSource=:memory:;Mode=Memory;Cache=Shared";
            _dbContext = new ApplicationDbContext(connectionString);

            _loggerMock = new Mock<ILogger<UnitOfWork>>();
            _unitOfWork = new UnitOfWork(_dbContext, _loggerMock.Object);
           
            InitializeDatabase();
            ConfigureDapperExtensions();
        }
        private void ConfigureDapperExtensions()
        {
            var dapperConfig = new DapperExtensionsConfiguration(
                typeof(DriverMap), // ✅ Register all mappings from this assembly
                new List<Assembly> { typeof(DriverMap).Assembly },
                new SqliteDialect() // ✅ Specify SQLite dialect explicitly
            );

            DapperExtensions.DapperExtensions.Configure(dapperConfig);
        }
        private void InitializeDatabase()
        {
            var sql = @"
            CREATE TABLE IF NOT EXISTS Driver (
                Id TEXT PRIMARY KEY,
                FirstName TEXT NOT NULL,
                LastName TEXT NOT NULL UNIQUE,
                Email TEXT NOT NULL UNIQUE,
                PhoneNumber TEXT NOT NULL
            );";

            _unitOfWork.GetConnection().Execute(sql);
        }

        [Fact]
        public async Task InsertAsync_ShouldInsertNewDriver()
        {
            // Arrange
            var driver = new Driver
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "+1234567890"
            };

            // Act
            _unitOfWork.BeginTransaction();
            await _unitOfWork.Repository<Driver>().InsertAsync(driver);
                await _unitOfWork.CommitAsync();

            var result = _unitOfWork.GetConnection().Get<Driver>(driver.Id);


            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(driver.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectDriver()
        {
            // Arrange
            var driver = new Driver
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                PhoneNumber = "+9876543210"
            };

            _unitOfWork.BeginTransaction();
            await _unitOfWork.Repository<Driver>().InsertAsync(driver);
            await _unitOfWork.CommitAsync();

            // Act
            var result = _unitOfWork.GetConnection().Get<Driver>(driver.Id);


            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(driver.Email);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingDriver()
        {
            // Arrange
            var driver = new Driver
            {
                Id = Guid.NewGuid(),
                FirstName = "Chris",
                LastName = "Evans",
                Email = "chris@example.com",
                PhoneNumber = "+3333333333"
            };

            _unitOfWork.BeginTransaction();
            await _unitOfWork.Repository<Driver>().InsertAsync(driver);
            await _unitOfWork.CommitAsync();

            // Act
            driver.FirstName = "Christian";
            driver.PhoneNumber = "+9999999999";

            _unitOfWork.BeginTransaction();
            await _unitOfWork.Repository<Driver>().UpdateAsync(driver);
            await _unitOfWork.CommitAsync();

            var updatedDriver =  _unitOfWork.GetConnection().Get<Driver>(driver.Id);

            // Assert
            updatedDriver.Should().NotBeNull();
            updatedDriver.FirstName.Should().Be("Christian");
            updatedDriver.PhoneNumber.Should().Be("+9999999999");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveDriver()
        {
            var id = Guid.NewGuid();
            // Arrange
            var driver = new Driver
            {
                Id = id,
                FirstName = "Daniel",
                LastName = "Craig",
                Email = $"{id}@example.com",
                PhoneNumber = "+4444444444"
            };

            _unitOfWork.BeginTransaction();
            await _unitOfWork.Repository<Driver>().InsertAsync(driver);
            await _unitOfWork.CommitAsync  ();

            // Act
            _unitOfWork.BeginTransaction();
            await _unitOfWork.Repository<Driver>().DeleteAsync(driver);
            await _unitOfWork.CommitAsync();
             
            var result = _unitOfWork.GetConnection().Get<Driver>(id);
           

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenDriverExists()
        {
            // Arrange
            var driver = new Driver
            {
                Id = Guid.NewGuid(),
                FirstName = "Emma",
                LastName = "Watson",
                Email = "emma@example.com",
                PhoneNumber = "+5555555555"
            };

            _unitOfWork.BeginTransaction();
            await _unitOfWork.Repository<Driver>().InsertAsync(driver);
           await  _unitOfWork.CommitAsync();

            // Act
            var exists = await _unitOfWork.Repository<Driver>().ExistsAsync(driver.Id);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task CountAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            var drivers = new List<Driver>
        {
            new Driver { Id = Guid.NewGuid(), FirstName = "Mike", LastName = "Tyson", Email = "mike@example.com", PhoneNumber = "+6666666666" },
            new Driver { Id = Guid.NewGuid(), FirstName = "Nina", LastName = "Dobrev", Email = "nina@example.com", PhoneNumber = "+7777777777" }
        };

            _unitOfWork.BeginTransaction();
            foreach (var driver in drivers)
            {
                await _unitOfWork.Repository<Driver>().InsertAsync(driver);
            }
           await _unitOfWork.CommitAsync();

            // Act
            var count = await _unitOfWork.Repository<Driver>().CountAsync();

            // Assert
            count.Should().Be(drivers.Count);
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}
