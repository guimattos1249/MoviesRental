using FluentValidation;
using MoviesRental.Core.ValidationMessages;

namespace MoviesRental.Queries.Application.UseCases.Dvds.Commands.UpdateDvd;

public class UpdateDvdCommandValidator : AbstractValidator<UpdateDvdCommand>
{
    public UpdateDvdCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(ValidationMessages.NOT_EMPTY_ERROR_MESSAGE);
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidationMessages.NOT_EMPTY_ERROR_MESSAGE);
        RuleFor(x => x.Published)
            .NotEmpty().WithMessage(ValidationMessages.ERROR_MESSAGE)
            .LessThan(DateTime.UtcNow).WithMessage(ValidationMessages.ERROR_MESSAGE);
        RuleFor(x => x.Genre)
            .NotEmpty().WithMessage(ValidationMessages.NOT_EMPTY_ERROR_MESSAGE);
        RuleFor(x => x.Copies)
            .GreaterThan(-1).WithMessage(ValidationMessages.ERROR_MESSAGE);
        RuleFor(x => x.DirectorId)
            .NotEmpty().WithMessage(ValidationMessages.ERROR_MESSAGE);
        RuleFor(x => x.UpdatedAt)
            .LessThan(DateTime.UtcNow).WithMessage(ValidationMessages.ERROR_MESSAGE);
    }
}
