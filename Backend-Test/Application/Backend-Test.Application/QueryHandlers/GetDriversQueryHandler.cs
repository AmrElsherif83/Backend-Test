using Backend_Test.Application.Queries;
using Backend_Test.Domain.Entities;
using Backend_Test.Domain.Interfaces;
using DapperExtensions.Predicate;
using DapperExtensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Test.Application.QueryHandlers
{
    public class GetDriversQueryHandler : IRequestHandler<GetDriversQuery, IEnumerable<Driver>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDriversQueryHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<Driver>> Handle(GetDriversQuery request, CancellationToken cancellationToken)
        {
            var predicateGroup = new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate>()
            };

            if (!string.IsNullOrWhiteSpace(request.FirstName))
            {
                predicateGroup.Predicates.Add(
                    Predicates.Field<Driver>(d => d.FirstName, Operator.Like, $"%{request.FirstName}%"));
            }

            if (!string.IsNullOrWhiteSpace(request.LastName))
            {
                predicateGroup.Predicates.Add(
                    Predicates.Field<Driver>(d => d.LastName, Operator.Like, $"%{request.LastName}%"));
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                predicateGroup.Predicates.Add(
                    Predicates.Field<Driver>(d => d.Email, Operator.Eq, request.Email));
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                predicateGroup.Predicates.Add(
                    Predicates.Field<Driver>(d => d.PhoneNumber, Operator.Eq, request.PhoneNumber));
            }

            IPredicate finalPredicate = predicateGroup.Predicates.Any()
                ? predicateGroup
                : null; // no filters means return all records

            var sorts = new List<ISort>
        {
            Predicates.Sort<Driver>(request.OrderBy ?? "LastName", request.OrderAscending)
        };

            return await _unitOfWork.Repository<Driver>().GetAllAsync(
                predicate: finalPredicate,
                sort: sorts,
                skip: request.Skip,
                take: request.Take);
        }
    }

}
