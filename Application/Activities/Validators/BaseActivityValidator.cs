using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities.DTOs;
using FluentValidation;

namespace Application.Activities.Validators
{
    public class BaseActivityValidator<T, TDto> : AbstractValidator<T> where TDto 
        : BaseActivityDto
    {
        public BaseActivityValidator(Func<T, TDto> selector)
        {
            RuleFor(x => selector(x).Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters");
            RuleFor(x => selector(x).Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
            RuleFor(x => selector(x).Date)
                .NotEmpty()
                .WithMessage("Date is required")
                .GreaterThan(DateTime.Now).WithMessage("Date must be in the future");
            RuleFor(x => selector(x).Category)
                .NotEmpty()
                .WithMessage("Category is required");
            RuleFor(x => selector(x).City)
                .NotEmpty()
                .WithMessage("City is required");
            RuleFor(x => selector(x).Venue)
                .NotEmpty()
                .WithMessage("Venue is required");
        }
    }
}