using ExpenseTrackerAPI.Models;
using FluentValidation;

namespace ExpenseTrackerAPI.Halpers
{
    public class UserModelValidator : AbstractValidator<UserModel>
    {
        public UserModelValidator()
        {
            // Validate UserName - Must not be empty
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

            // Validate Email - Must be a valid email format
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address format.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            // Validate Password - Must not be empty and should meet strength criteria
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
            //.Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            //.Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            //.Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            //.Matches(@"[\W]").WithMessage("Password must contain at least one special character (e.g., !@#$%).");

            // Validate Address - Must not be empty or null
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");
        }
    }

}
