using Backend_Test.Application.Queries;
using Backend_Test.Domain.Common.Exceptions;
using Backend_Test.Domain.Entities;
using Backend_Test.Domain.Interfaces;
using DapperExtensions.Predicate;
using DapperExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Application.QueryHandlers
{
    public class GetDriverByEmailQueryHandler : IRequestHandler<GetDriverByEmailQuery, Driver>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetDriverByEmailQueryHandler> _logger;

        public GetDriverByEmailQueryHandler(IUnitOfWork unitOfWork, ILogger<GetDriverByEmailQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Driver> Handle(GetDriverByEmailQuery request, CancellationToken cancellationToken)
        {
            var predicate = Predicates.Field<Driver>(d => d.Email, Operator.Eq, request.Email);
            var result = await _unitOfWork.Repository<Driver>().GetAllAsync(predicate);

            var driver = result.SingleOrDefault();

            if (driver == null)
            {
                _logger.LogWarning("Driver with Email={Email} not found.", request.Email);
                throw new NotFoundException(nameof(Driver), request.Email);
            }

            _logger.LogInformation("Successfully retrieved driver with Email={Email}", request.Email);
            return driver;
        }
    }
}
