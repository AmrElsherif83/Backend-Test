using Backend_Test.Application.Commands;
using Backend_Test.Domain.Common.Exceptions;
using Backend_Test.Domain.Entities;
using Backend_Test.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace Backend_Test.Application.CommandHandlers
{
    public class UpdateDriverCommandHandler : IRequestHandler<UpdateDriverCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateDriverCommand> _validator;

        public UpdateDriverCommandHandler(IUnitOfWork unitOfWork, IValidator<UpdateDriverCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task Handle(UpdateDriverCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            _unitOfWork.BeginTransaction();

            var repo = _unitOfWork.Repository<Driver>();
            var driver = await repo.GetByIdAsync(request.Id);

            if (driver == null)
            {
                _unitOfWork.Rollback();
                throw new NotFoundException(nameof(Driver), request.Id);
            }

            driver.FirstName = request.FirstName;
            driver.LastName = request.LastName;
            driver.Email = request.Email;
            driver.PhoneNumber = request.PhoneNumber;

            await repo.UpdateAsync(driver);
            await _unitOfWork.CommitAsync();
        }
    }

}
