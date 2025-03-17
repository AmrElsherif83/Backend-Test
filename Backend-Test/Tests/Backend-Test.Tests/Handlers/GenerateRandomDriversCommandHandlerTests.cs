using Backend_Test.Application.CommandHandlers;
using Backend_Test.Application.Commands;
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
    public class GenerateRandomDriversCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Driver>> _mockRepository;
        private readonly GenerateRandomDriversCommandHandler _handler;

        public GenerateRandomDriversCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IGenericRepository<Driver>>();

            _mockUnitOfWork.Setup(uow => uow.Repository<Driver>()).Returns(_mockRepository.Object);
            _handler = new GenerateRandomDriversCommandHandler(_mockUnitOfWork.Object, new NullLogger<GenerateRandomDriversCommandHandler>());
        }

        [Fact]
        public async Task Handle_ShouldGenerateAndInsertTenRandomDrivers()
        {
            // Arrange
            var command = new GenerateRandomDriversCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().HaveCount(10);
            _mockRepository.Verify(repo => repo.InsertAsync(It.IsAny<Driver>()), Times.Exactly(10));
        }
    }
}
