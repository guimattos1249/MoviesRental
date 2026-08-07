using FluentValidation;
using MoviesRental.Core.ValidationMessages;

namespace MoviesRental.Queries.Application.UseCases.Directors.Commands.UpdateDirector;

public class UpdateDirectorCommandValidator : AbstractValidator<UpdateDirectorCommand>
{
    private const int MIN_LENGTH = 3;
    private const int MAX_LENGTH = 100;
    public UpdateDirectorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(ValidationMessages.NOT_EMPTY_ERROR_MESSAGE);
        RuleFor(d => d.FullName)
            .NotEmpty().WithMessage(ValidationMessages.NOT_EMPTY_ERROR_MESSAGE)
            .MinimumLength(MIN_LENGTH).WithMessage(ValidationMessages.MIN_LENGTH_ERROR_MESSAGE)
            .MaximumLength(MAX_LENGTH).WithMessage(ValidationMessages.MAX_LENGTH_ERROR_MESSAGE);
        RuleFor(d => d.UpdatedAt)
            .LessThan(DateTime.UtcNow).WithMessage(ValidationMessages.ERROR_MESSAGE);
    }
}
