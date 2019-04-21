using FluentValidation;
using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Validations
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordModel>
    {
        public ChangePasswordValidator()
        {
            RuleFor(u => u.UserId)
               .GreaterThan(0)
               .WithMessage("User Id Should Be Greater Then Zero");
            RuleFor(u => u.CurrentPassword)
                .NotNull()
                .NotEmpty()
                .WithMessage("Please Enter Current Password")
                .MaximumLength(50)
                .WithMessage("Current Password Max Length Max Length Should Be 50");
            RuleFor(u => u.NewPassword)
                .NotNull()
                .NotEmpty()
                .WithMessage("Please Enter New Password")
                .MaximumLength(50)
                .WithMessage("Current Password Max Length Max Length Should Be 50");
            RuleFor(u => u.NewPasswordConfirmation)
                .NotNull()
                .NotEmpty()
                .WithMessage("Please Enter New Password Confirmation")
                .MaximumLength(50)
                .WithMessage("New Password Confirmation Max Length Should Be 50")
                .Equal(x => x.NewPassword)
                .WithMessage("Passwords Do Not Match");
        }
    }
}
