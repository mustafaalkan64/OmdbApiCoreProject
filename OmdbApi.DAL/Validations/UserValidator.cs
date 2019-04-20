using FluentValidation;
using OmdbApi.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Validations
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(u => u.FirstName)
                .NotNull()
                .NotEmpty()
                .WithMessage("FirstName Is Required")
                .MaximumLength(50)
                .WithMessage("First Name Max Length Can Not Be Over 50"); 
            RuleFor(u => u.LastName)
                .NotNull()
                .NotEmpty()
                .WithMessage("LastName Is Required")
                .MaximumLength(50)
                .WithMessage("Last Name Max Length Can Not Be Over 50");
            RuleFor(u => u.Username)
                .NotNull()
                .NotEmpty()
                .WithMessage("UserName Is Required")
                .MaximumLength(50)
                .WithMessage("Last Name Max Length Can Not Be Over 50");
            RuleFor(u => u.Password)
                .NotNull()
                .NotEmpty()
                .WithMessage("Password Is Required")
                .MaximumLength(50)
                .WithMessage("Password Max Length Can Not Be Over 50");
            RuleFor(u => u.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage("Email Is Required")
                .MaximumLength(150)
                .WithMessage("Email Max Length Can Not Be Over 150")
                .EmailAddress()
                .WithMessage("Email Format Is Invalid");
        }
    }
}
