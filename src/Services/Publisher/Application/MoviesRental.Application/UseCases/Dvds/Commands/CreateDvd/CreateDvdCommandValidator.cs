using FluentValidation;
using MoviesRental.Core.ValidationMessages;
using MoviesRental.Domain.Entities;

namespace MoviesRental.Application.UseCases.Dvds.Commands.CreateDvd;

public class CreateDvdCommandValidator : AbstractValidator<CreateDvdCommand>
{
    private const string GENRE_ERROR_MESSAGE = "Invalid genre type";
    private const int GENRE_ERROR_NUMBER = 19;
    private const int COPIES_ERROR_NUMBER = -1;
    public CreateDvdCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidationMessages.NOT_EMPTY_ERROR_MESSAGE)
            .MinimumLength(Dvd.MinTitleLength).WithMessage(ValidationMessages.MIN_LENGTH_ERROR_MESSAGE)
            .MaximumLength(Dvd.MaxTitleLength).WithMessage(ValidationMessages.MAX_LENGTH_ERROR_MESSAGE);
        RuleFor(x => x.Genre)
            .LessThan(GENRE_ERROR_NUMBER).WithMessage(GENRE_ERROR_MESSAGE);
        RuleFor(x => x.Published)
            .LessThan(DateTime.UtcNow).WithMessage(ValidationMessages.ERROR_MESSAGE);
        RuleFor(x => x.Copies)
            .GreaterThan(COPIES_ERROR_NUMBER).WithMessage(ValidationMessages.ERROR_MESSAGE);
        RuleFor(x => x.DirectorId)
            .NotEqual(Guid.Empty).WithMessage(ValidationMessages.ERROR_MESSAGE);
    }
}
