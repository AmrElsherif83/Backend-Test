using Backend_Test.Application.Commands;
using Backend_Test.Domain.Common.Exceptions;
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
    public class DeleteDriverCommandHandler : IRequestHandler<DeleteDriverCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteDriverCommandHandler> _logger;

        public DeleteDriverCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteDriverCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeleteDriverCommand request, CancellationToken cancellationToken)
        {
            _unitOfWork.BeginTransaction();
            var repo = _unitOfWork.Repository<Driver>();

            var driver = await repo.GetByIdAsync(request.Id);

            if (driver is null)
            {
                _logger.LogWarning("Delete failed. Driver with Id={DriverId} not found.", request.Id);
                _unitOfWork.Rollback();
                throw new NotFoundException(nameof(Driver), request.Id);
            }

            await repo.DeleteAsync(driver);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Driver with Id={DriverId} deleted successfully.", request.Id);
        }
    }
}
