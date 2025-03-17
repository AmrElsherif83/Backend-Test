using Backend_Test.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Application.Commands
{
    public record GenerateRandomDriversCommand : IRequest<IEnumerable<Driver>>;

}
