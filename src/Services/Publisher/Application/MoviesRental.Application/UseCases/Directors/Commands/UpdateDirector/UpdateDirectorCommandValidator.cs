using FluentValidation;
using MoviesRental.Core.ValidationMessages;
using MoviesRental.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesRental.Application.UseCases.Directors.Commands.UpdateDirector;

public class UpdateDirectorCommandValidator : AbstractValidator<UpdateDirectorCommand>
{
    public UpdateDirectorCommandValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty).WithMessage(ValidationMessages.NOT_EMPTY_ERROR_MESSAGE);
        RuleFor(d => d.Name)
            .NotEmpty().WithMessage(ValidationMessages.NOT_EMPTY_ERROR_MESSAGE)
            .MinimumLength(Director.MinNameLength).WithMessage(ValidationMessages.MIN_LENGTH_ERROR_MESSAGE)
            .MaximumLength(Director.MaxNameLength).WithMessage(ValidationMessages.MAX_LENGTH_ERROR_MESSAGE);
        RuleFor(d => d.Surname)
            .NotEmpty().WithMessage(ValidationMessages.NOT_EMPTY_ERROR_MESSAGE)
            .MinimumLength(Director.MinNameLength).WithMessage(ValidationMessages.MIN_LENGTH_ERROR_MESSAGE)
            .MaximumLength(Director.MaxNameLength).WithMessage(ValidationMessages.MAX_LENGTH_ERROR_MESSAGE);
    }
}
