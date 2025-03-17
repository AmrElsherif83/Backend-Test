using Backend_Test.Application.CommandHandlers;
using Backend_Test.Application.Commands;
using Backend_Test.Domain.Entities;
using Backend_Test.Domain.Interfaces;
using FluentAssertions;
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
    public class CreateDriverCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Driver>> _mockRepository;
        private readonly CreateDriverCommandHandler _handler;
        private readonly Mock<IValidator<CreateDriverCommand>> _mockValidator;
        public CreateDriverCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IGenericRepository<Driver>>();
            _mockValidator = new Mock<IValidator<CreateDriverCommand>>();
            _mockUnitOfWork.Setup(uow => uow.Repository<Driver>()).Returns(_mockRepository.Object);
            _handler = new CreateDriverCommandHandler(_mockUnitOfWork.Object, _mockValidator.Object, new NullLogger<CreateDriverCommandHandler>());
        }

        [Fact]
        public async Task Handle_ShouldCreateNewDriver()
        {
            // Arrange
            var command = new CreateDriverCommand("John", "Doe", "john@example.com", "+1234567890");

            var driverId = Guid.NewGuid();
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateDriverCommand>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new ValidationResult());

            _mockRepository.Setup(repo => repo.InsertAsync(It.IsAny<Driver>()))
                           .Callback<Driver>(d => d.Id = driverId)
                           .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.BeginTransaction());
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(driverId);
            _mockRepository.Verify(repo => repo.InsertAsync(It.IsAny<Driver>()), Times.Once);  // <-- Ensures it's called only once
            _mockUnitOfWork.Verify(u => u.BeginTransaction(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenInvalidDataProvided()
        {
            // Arrange
            var command = new CreateDriverCommand("John", "Doe", "invalid-email", "+1234567890");

            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateDriverCommand>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
                          {
                          new ValidationFailure("Email", "Invalid email format")
                          }));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenDriverAlreadyExists()
        {
            // Arrange
            var command = new CreateDriverCommand("John", "Doe", "john@example.com", "+1234567890");
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateDriverCommand>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new ValidationResult());
            _mockRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true); // Simulating duplicate record

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
