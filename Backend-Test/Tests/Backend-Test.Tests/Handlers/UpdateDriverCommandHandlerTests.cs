using Backend_Test.Application.CommandHandlers;
using Backend_Test.Application.Commands;
using Backend_Test.Domain.Common.Exceptions;
using Backend_Test.Domain.Entities;
using Backend_Test.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Tests.Handlers
{
    public class UpdateDriverCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Driver>> _mockRepository;
        private readonly Mock<IValidator<UpdateDriverCommand>> _mockValidator;
        private readonly UpdateDriverCommandHandler _handler;
        public UpdateDriverCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IGenericRepository<Driver>>();
            _mockValidator = new Mock<IValidator<UpdateDriverCommand>>();

            _mockUnitOfWork.Setup(uow => uow.Repository<Driver>()).Returns(_mockRepository.Object);
            _handler = new UpdateDriverCommandHandler(_mockUnitOfWork.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateDriver_WhenDriverExists()
        {
            // Arrange
            var driverId = Guid.NewGuid();
            var existingDriver = new Driver { Id = driverId, FirstName = "John", LastName = "Doe", Email = "john@example.com", PhoneNumber = "+1234567890" };
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<UpdateDriverCommand>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(new ValidationResult());
            _mockRepository.Setup(repo => repo.GetByIdAsync(driverId)).ReturnsAsync(existingDriver);
            _mockUnitOfWork.Setup(u => u.BeginTransaction());
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            var command = new UpdateDriverCommand(
                driverId,
                "UpdatedFirst",
                "UpdatedLast",
                "updated@example.com",
                "+9876543210"
            );

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateAsync(It.Is<Driver>(d =>
                d.Id == driverId &&
                d.FirstName == "UpdatedFirst" &&
                d.LastName == "UpdatedLast" &&
                d.Email == "updated@example.com" &&
                d.PhoneNumber == "+9876543210"
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenDriverDoesNotExist()
        {
            // Arrange
            var driverId = Guid.NewGuid();
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<UpdateDriverCommand>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new ValidationResult());
            _mockRepository.Setup(repo => repo.GetByIdAsync(driverId)).ReturnsAsync((Driver)null);
            _mockUnitOfWork.Setup(u => u.BeginTransaction());
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            var command = new UpdateDriverCommand(
                driverId,
                "UpdatedFirst",
                "UpdatedLast",
                "updated@example.com",
                "+9876543210"
            );

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
