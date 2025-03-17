using Backend_Test.Application.DTOS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Application.Queries
{
    public record GetDriverNameAlphabetizedQuery(Guid Id) : IRequest<DriverAlphabetizedNameDto>;
}
