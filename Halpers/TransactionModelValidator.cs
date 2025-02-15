using ExpenseTrackerAPI.Models;
using FluentValidation;

namespace ExpenseTrackerAPI.Halpers
{
    public class TransactionModelValidator : AbstractValidator<TransactionModel>
    {
        public TransactionModelValidator()
        {
            // Validate UserID - Required
            RuleFor(x => x.UserID)
                .NotNull().WithMessage("UserID is required.");

            // Validate CategoryID - Required
            RuleFor(x => x.CategoryID)
                .NotNull().WithMessage("CategoryID is required.");

            // Validate Date - Must be a valid date in the past
            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Date cannot be in the future.");

            // Validate Amount - Must be a positive number
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");
        }
    }
}
