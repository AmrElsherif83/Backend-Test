using Backend_Test.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Application.Queries
{
    public record GetDriversQuery : IRequest<IEnumerable<Driver>>
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }

        public string OrderBy { get; init; } = "Id";
        public bool OrderAscending { get; init; } = true;
        public int Skip { get; init; } = 0;
        public int Take { get; init; } = 10;
    }

}
