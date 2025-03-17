using Backend_Test.Application.DTOS;
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
    public class GetDriverNameAlphabetizedQueryHandler : IRequestHandler<GetDriverNameAlphabetizedQuery, DriverAlphabetizedNameDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDriverNameAlphabetizedQueryHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<DriverAlphabetizedNameDto> Handle(GetDriverNameAlphabetizedQuery request, CancellationToken cancellationToken)
        {
            var driver = await _unitOfWork.Repository<Driver>().GetByIdAsync(request.Id);

            if (driver == null)
                return null;

            var alphabetizedFirstName = AlphabetizeString(driver.FirstName);
            var alphabetizedLastName = AlphabetizeString(driver.LastName);

            var original = $"{driver.FirstName} {driver.LastName}";
            var alphabetized = $"{alphabetizedFirstName} {alphabetizedLastName}";

            return new DriverAlphabetizedNameDto(original, alphabetized);
        }

        private static string AlphabetizeString(string input)
        {
            return string.Concat(input.OrderBy(char.ToLower).ThenBy(c => c));
        }
    }
}
