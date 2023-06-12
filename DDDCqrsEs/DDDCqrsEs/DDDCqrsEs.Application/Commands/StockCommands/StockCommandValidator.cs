using FluentValidation;
using DDDCqrsEs.Domain.Repositories;
using System;

namespace DDDCqrsEs.Application.Commands.StockCommands
{
    public class CreateStockCommandValidator : AbstractValidator<CreateStockCommand>
    {
        private readonly IStockProjectionRepository _stockProjectionRepository;

        public CreateStockCommandValidator(IStockProjectionRepository stockProjectionRepository)
        {
            _stockProjectionRepository = stockProjectionRepository;
            RuleFor(x => x.BestBeforeDate).Must(IsNotInPast).WithMessage("You cannot enter a date from the past.");
            RuleFor(x => x.LicensePlate).Must(BeUnique).WithMessage("License plate already exists in database.");
        }

        private bool BeUnique(string licensePlate)
        {
            var stockWithLicensePlate = _stockProjectionRepository.GetStockByLicensePlate(licensePlate);
            if (stockWithLicensePlate == null)
            {
                return true;
            }
            return false;
        }

        public bool IsNotInPast(DateTime date)
        {
            var datesCompared = DateTime.Compare(date, DateTime.Now);
            if (datesCompared < 0)
            {
                return false;
            }
            return true;
        }
    }
    public class UpdateStockCommandValidator : AbstractValidator<UpdateStockCommand>
    {
        private readonly IStockProjectionRepository _stockProjectionRepository;

        public UpdateStockCommandValidator(IStockProjectionRepository stockProjectionRepository)
        {
            _stockProjectionRepository = stockProjectionRepository;
            RuleFor(x => x.BestBeforeDate).Must(IsNotInPast).WithMessage("You cannot enter a date from the past.");
            RuleFor(x => x).Must(BeUnique).WithMessage("License plate already exists in database.");
            RuleFor(x => x).Must(HasNotChangedSince).WithMessage("Data was changed by another user.");
        }
         
        private bool BeUnique(UpdateStockCommand command)
        {
            var stockWithLicensePlate = _stockProjectionRepository.GetStockByLicensePlate(command.LicensePlate);
            if (stockWithLicensePlate == null || stockWithLicensePlate.Id == command.Id)
            {
                return true;
            }
            return false;
        }

        public bool IsNotInPast(DateTime date)
        {
            var datesCompared = DateTime.Compare(date, DateTime.Now);
            if (datesCompared < 0)
            {
                return false;
            }
            return true;
        }

        public bool HasNotChangedSince(UpdateStockCommand command)
        {
            var stockFromDb = _stockProjectionRepository.GetStockById(command.Id);
            if (stockFromDb.Version == command.Version) {
                return true;
            }
            return false;
        }
    }
    public class DeleteStockCommandValidator : AbstractValidator<DeleteStockCommand>
    {
        private readonly IStockProjectionRepository _stockProjectionRepository;

        public DeleteStockCommandValidator(IStockProjectionRepository stockProjectionRepository)
        {
            _stockProjectionRepository = stockProjectionRepository;
        }

        public bool HasNotChangedSince(DeleteStockCommand command)
        {
            var stockFromDb = _stockProjectionRepository.GetStockById(command.Id);
            if (stockFromDb.Version == command.Version)
            {
                return true;
            }
            return false;
        }
    }

}
