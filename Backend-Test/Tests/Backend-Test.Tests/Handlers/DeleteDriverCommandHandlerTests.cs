using Backend_Test.Application.CommandHandlers;
using Backend_Test.Application.Commands;
using Backend_Test.Domain.Common.Exceptions;
using Backend_Test.Domain.Entities;
using Backend_Test.Domain.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Tests.Handlers
{
    public class DeleteDriverCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Driver>> _mockRepository;
        private readonly DeleteDriverCommandHandler _handler;

        public DeleteDriverCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IGenericRepository<Driver>>();

            _mockUnitOfWork.Setup(uow => uow.Repository<Driver>()).Returns(_mockRepository.Object);
            _handler = new DeleteDriverCommandHandler(_mockUnitOfWork.Object, new NullLogger<DeleteDriverCommandHandler>());
        }

        [Fact]
        public async Task Handle_ShouldDeleteDriver_WhenDriverExists()
        {
            // Arrange
            var driverId = Guid.NewGuid();
            var driver = new Driver { Id = driverId, FirstName = "John", LastName = "Doe" };
            _mockRepository.Setup(repo => repo.GetByIdAsync(driverId)).ReturnsAsync(driver);

            var command = new DeleteDriverCommand(driverId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteAsync(driver), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenDriverDoesNotExist()
        {
            // Arrange
            var driverId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(driverId)).ReturnsAsync((Driver)null);

            var command = new DeleteDriverCommand(driverId);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
