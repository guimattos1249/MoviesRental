using FluentValidation;
using MoviesRental.Core.ValidationMessages;
using MoviesRental.Domain.Entities;

namespace MoviesRental.Application.UseCases.Dvds.Commands.UpdateDvd;

public class UpdateDvdCommandValidator : AbstractValidator<UpdateDvdCommand>
{
    private const string GENRE_ERROR_MESSAGE = "Invalid genre type";
    private const int GENRE_ERROR_NUMBER = 19;
    private const int COPIES_ERROR_NUMBER = -1;
    public UpdateDvdCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage(ValidationMessages.ERROR_MESSAGE);
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidationMessages.NOT_EMPTY_ERROR_MESSAGE)
            .MinimumLength(Dvd.MinTitleLength).WithMessage(ValidationMessages.MIN_LENGTH_ERROR_MESSAGE)
            .MaximumLength(Dvd.MaxTitleLength).WithMessage(ValidationMessages.MAX_LENGTH_ERROR_MESSAGE);
        RuleFor(x => x.Genre)
            .LessThan(GENRE_ERROR_NUMBER).WithMessage(GENRE_ERROR_MESSAGE)
            .GreaterThan(COPIES_ERROR_NUMBER).WithMessage(GENRE_ERROR_MESSAGE);
        RuleFor(x => x.Published)
            .LessThan(DateTime.UtcNow).WithMessage(ValidationMessages.ERROR_MESSAGE);
        RuleFor(x => x.Copies)
            .GreaterThanOrEqualTo(COPIES_ERROR_NUMBER).WithMessage(ValidationMessages.ERROR_MESSAGE);
        RuleFor(x => x.DirectorId)
            .NotEqual(Guid.Empty).WithMessage(ValidationMessages.ERROR_MESSAGE);
    }
}
