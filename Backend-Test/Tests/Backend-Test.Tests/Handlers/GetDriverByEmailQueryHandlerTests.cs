using Backend_Test.Application.Queries;
using Backend_Test.Application.QueryHandlers;
using Backend_Test.Domain.Common.Exceptions;
using Backend_Test.Domain.Interfaces;
using DapperExtensions.Predicate;
using DapperExtensions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend_Test.Domain.Entities;
using FluentAssertions;

namespace Backend_Test.Tests.Handlers
{
    public class GetDriverByEmailQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Driver>> _mockRepository;
        private readonly GetDriverByEmailQueryHandler _handler;

        public GetDriverByEmailQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IGenericRepository<Driver>>();

            _mockUnitOfWork.Setup(uow => uow.Repository<Driver>()).Returns(_mockRepository.Object);
            _handler = new GetDriverByEmailQueryHandler(_mockUnitOfWork.Object, new NullLogger<GetDriverByEmailQueryHandler>());
        }

        [Fact]
        public async Task Handle_ShouldReturnDriver_WhenDriverExists()
        {
            // Arrange
            var email = "johndoe@example.com";
            var driver = new Driver { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = email };

            var predicate = Predicates.Field<Driver>(d => d.Email, Operator.Eq, email);
            _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<IPredicate>(), null, null, null))
     .ReturnsAsync(new List<Driver> { driver });

            var query = new GetDriverByEmailQuery(email);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(email);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenDriverDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var predicate = Predicates.Field<Driver>(d => d.Email, Operator.Eq, email);
            _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<IPredicate>(), null, null, null))
      .ReturnsAsync(new List<Driver>());

            var query = new GetDriverByEmailQuery(email);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
