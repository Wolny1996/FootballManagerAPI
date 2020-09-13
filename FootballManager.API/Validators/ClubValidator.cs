using FluentValidation;
using FootballManager.API.DTOs.Command;
using System;

namespace FootballManager.API.Validators
{
    public class ClubValidator : AbstractValidator<ClubCommand>
    {
        public ClubValidator()
        {
            RuleFor(x => x.ClubName).NotEmpty()
                .Must(c => Char.IsUpper(c[0]))
                .WithMessage("The club's name must be in upper case");
            
            RuleFor(x => x.City).NotEmpty()
                .Must(c => Char.IsUpper(c[0]))
                .WithMessage("The city's name must be in upper case");

            RuleFor(x => x.Founded).NotEmpty();
            RuleFor(f => f.Founded.Year).GreaterThan(1857).LessThanOrEqualTo(DateTime.Today.Year);
            RuleFor(f => f.Founded.Month).GreaterThan(0).LessThanOrEqualTo(12);
            RuleFor(f => f.Founded.Day).GreaterThan(0).LessThanOrEqualTo(31);
        }
    }
}