using ExpenseTrackerAPI.Models;
using FluentValidation;

namespace ExpenseTrackerAPI.Halpers
{
    public class CategoryModelValidator : AbstractValidator<CategoryModel>
    {
        public CategoryModelValidator()
        {
            // Validate UserID (nullable int)
            RuleFor(x => x.UserID)
                .NotNull().WithMessage("UserID is required.");

            // Validate CategoryName (non-empty and length check)
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Category Name is required.")
                .Length(3, 50).WithMessage("Category Name must be between 3 and 50 characters.");

            // Validate Icon (non-empty if provided, length check)
            //RuleFor(x => x.Icon)
            //    .NotEmpty().When(x => !string.IsNullOrEmpty(x.Icon)).WithMessage("Icon cannot be empty when provided.")
            //    .MaximumLength(100).WithMessage("Icon must not exceed 100 characters.");
            RuleFor(x => x.Icon)
              .NotNull().WithMessage("Icon is required.");

            // Validate Type (non-empty and length check)
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.");
        }
    }
}
