using Backend_Test.Application.Queries;
using Backend_Test.Domain.Entities;
using Backend_Test.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Application.QueryHandlers
{
    public class GetAlphabetizedDriversQueryHandler : IRequestHandler<GetAlphabetizedDriversQuery, IEnumerable<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAlphabetizedDriversQueryHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<string>> Handle(GetAlphabetizedDriversQuery request, CancellationToken cancellationToken)
        {
            var drivers = await _unitOfWork.Repository<Driver>().GetAllAsync();

            return drivers
                .Select(d => $"{d.FirstName} {d.LastName}")
                .OrderBy(fullName => fullName)
                .ToList();
        }
    }

}
