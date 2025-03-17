using Backend_Test.Application.Queries;
using Backend_Test.Application.QueryHandlers;
using Backend_Test.Domain.Common.Exceptions;
using Backend_Test.Domain.Entities;
using Backend_Test.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Tests.Handlers
{
    public class GetDriverByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Driver>> _mockRepository;
        private readonly GetDriverByIdQueryHandler _handler;

        public GetDriverByIdQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IGenericRepository<Driver>>();

            _mockUnitOfWork.Setup(uow => uow.Repository<Driver>()).Returns(_mockRepository.Object);
            _handler = new GetDriverByIdQueryHandler(_mockUnitOfWork.Object, new NullLogger<GetDriverByIdQueryHandler>());
        }

        [Fact]
        public async Task Handle_ShouldReturnDriver_WhenDriverExists()
        {
            // Arrange
            var driverId = Guid.NewGuid();
            var driver = new Driver { Id = driverId, FirstName = "John", LastName = "Doe" };
            _mockRepository.Setup(repo => repo.GetByIdAsync(driverId)).ReturnsAsync(driver);

            var query = new GetDriverByIdQuery(driverId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(driverId);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenDriverDoesNotExist()
        {
            // Arrange
            var driverId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(driverId)).ReturnsAsync((Driver)null);

            var query = new GetDriverByIdQuery(driverId);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
