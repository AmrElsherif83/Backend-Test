﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Application.Commands
{
    public record DeleteDriverCommand(Guid Id) : IRequest;

}
