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
            RuleFor(u => u.FirstName).NotEmpty().MinimumLength(4).MaximumLength(32);
            RuleFor(u => u.LastName).NotEmpty().MinimumLength(4).MaximumLength(32);
            RuleFor(u => u.Username).NotEmpty().MinimumLength(4).MaximumLength(32);
            RuleFor(u => u.Email).NotEmpty().MinimumLength(4).MaximumLength(128).EmailAddress();
        }
    }
}
