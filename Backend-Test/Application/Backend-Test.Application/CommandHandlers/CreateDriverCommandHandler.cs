using Backend_Test.Application.Commands;
using Backend_Test.Domain.Entities;
using Backend_Test.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Application.CommandHandlers
{
    public class CreateDriverCommandHandler : IRequestHandler<CreateDriverCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateDriverCommand> _validator;
        private readonly ILogger<CreateDriverCommandHandler> _logger;
        public CreateDriverCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateDriverCommand> validator, ILogger<CreateDriverCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateDriverCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting CreateDriverCommandHandler for {Email}", request.Email);

            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            if (!_unitOfWork.IsTransactionActive)
            {
                _unitOfWork.BeginTransaction();
            }
            try
            {
                var driver = new Driver
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber
                };

                if (! await _unitOfWork.Repository<Driver>().ExistsAsync(driver.Id))
                {
                    await _unitOfWork.Repository<Driver>().InsertAsync(driver);
                }
                else
                {
                    throw new InvalidOperationException("Driver with this ID already exists.");
                }
                await _unitOfWork.CommitAsync();
                _logger.LogInformation("Driver created successfully: {DriverId}", driver.Id);
                return driver.Id;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError(ex, "Error creating driver: {Email}", request.Email);
                throw;
            }

        }
    }

}
