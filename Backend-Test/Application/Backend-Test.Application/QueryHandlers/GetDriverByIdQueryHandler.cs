using Backend_Test.Application.Queries;
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

namespace Backend_Test.Application.QueryHandlers
{
    public class GetDriverByIdQueryHandler : IRequestHandler<GetDriverByIdQuery, Driver>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetDriverByIdQueryHandler> _logger;

        public GetDriverByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetDriverByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Driver> Handle(GetDriverByIdQuery request, CancellationToken cancellationToken)
        {
            var driver = await _unitOfWork.Repository<Driver>().GetByIdAsync(request.Id);

            if (driver == null)
            {
                _logger.LogWarning("Driver with Id={DriverId} not found.", request.Id);
                throw new NotFoundException(nameof(Driver), request.Id);
            }

            _logger.LogInformation("Successfully retrieved driver with Id={DriverId}", request.Id);
            return driver;
        }
    }
}
