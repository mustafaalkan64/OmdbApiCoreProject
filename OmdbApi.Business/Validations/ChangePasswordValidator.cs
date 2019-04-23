using FluentValidation;
using OmdbApi.DAL.Models;

namespace OmdbApi.Business.Validations
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordModel>
    {
        public ChangePasswordValidator()
        {
            RuleFor(u => u.UserId)
               .GreaterThan(0)
               .WithMessage("User Id Should Be Greater Then Zero");
            RuleFor(u => u.CurrentPassword)
                .NotEmpty()
                .WithMessage("Please Enter Current Password");
            RuleFor(u => u.NewPassword)
                .NotEmpty()
                .WithMessage("Please Enter New Password")
                .MaximumLength(50)
                .WithMessage("New Password Max Length Should Be 50")
                .MinimumLength(8)
                .WithMessage("New Password Minimum Length Should Be 8");
            RuleFor(u => u.NewPasswordConfirmation)
                .NotEmpty()
                .WithMessage("Please Enter New Password Confirmation")
                .MaximumLength(50)
                .WithMessage("New Password Confirmation Max Length Should Be 50")
                .Equal(x => x.NewPassword)
                .WithMessage("Passwords Do Not Match");
        }
    }
}
