using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Application.Commands
{
    public record UpdateDriverCommand(Guid Id, string FirstName, string LastName, string Email, string PhoneNumber)
    : IRequest;
}
