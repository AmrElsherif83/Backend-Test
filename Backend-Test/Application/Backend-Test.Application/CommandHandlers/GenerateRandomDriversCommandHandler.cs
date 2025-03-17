using Backend_Test.Application.Commands;
using Backend_Test.Domain.Entities;
using Backend_Test.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Application.CommandHandlers
{
    public class GenerateRandomDriversCommandHandler : IRequestHandler<GenerateRandomDriversCommand, IEnumerable<Driver>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GenerateRandomDriversCommandHandler> _logger;

        public GenerateRandomDriversCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<GenerateRandomDriversCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Driver>> Handle(GenerateRandomDriversCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<Driver>();
            var drivers = new List<Driver>();

            _unitOfWork.BeginTransaction();

            try
            {
                for (var i = 0; i < 10; i++)
                {
                    var driver = new Driver
                    {
                        Id = Guid.NewGuid(),
                        FirstName = GenerateRandomString(6),
                        LastName = GenerateRandomString(8),
                        Email = $"{GenerateRandomString(5)}@example.com",
                        PhoneNumber = $"+{new Random().Next(10000000, 999999999)}"
                    };

                    await repo.InsertAsync(driver);
                    drivers.Add(driver);
                }

                await _unitOfWork.CommitAsync();
                _logger.LogInformation("Generated and inserted 10 random drivers.");

                return drivers;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError(ex, "Error generating random drivers");
                throw;
            }
        }

        private static string GenerateRandomString(int length)
        {
            var random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

}
