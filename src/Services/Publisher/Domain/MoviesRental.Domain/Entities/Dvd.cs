using MoviesRental.Core.DomainObjects;
using MoviesRental.Domain.Entities.Enums;

namespace MoviesRental.Domain.Entities;

public class Dvd : Entity
{
    public Dvd() { }

    public Dvd(string title, int genre, DateTime published, int copies, Guid directorId)
    {
        Available = true;
        UpdateTitle(title);
        UpdateGenre(genre);
        UpdatePublishedDate(published);
        UpdateCopies(copies);
        UpdateDirector(directorId);
    }

    public string Title { get; private set; }
    public EGenre Genre { get; private set; }
    public DateTime Published { get; private set; }
    public bool Available { get; private set; }
    public int Copies { get; private set; }
    public Guid DirectorId { get; private set; }
    public Director Director { get; set; }

    public const int MaxTitleLength = 100;
    public const int MinTitleLength = 3;

    public void IsAvailable()
    {
        if(!Available)
            throw new DomainException($"Dvd {Title} is not available for return.");
    }

    public void RentCopy()
    {
        if (Copies == 0 || !Available)
            throw new DomainException($"No copies available for rent to Dvd {Title}.");

        var copies = Copies - 1;
        UpdateCopies(copies);
    }

    public void ReturnCopy()
    {
        IsAvailable();

        var copies = Copies + 1;
        UpdateCopies(copies);
    }

    public void UpdateTitle(string title)
    {
        IsAvailable();

        if (string.IsNullOrWhiteSpace(title) || title.Length < MinTitleLength || title.Length > MaxTitleLength)
            throw new DomainException($"Dvd {title} is invalid.");

        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateGenre(int genre)
    {
        IsAvailable();

        Genre = genre switch
        {
            0 => EGenre.Action,
            1 => EGenre.Adventure,
            2 => EGenre.Animation,
            3 => EGenre.Comedy,
            4 => EGenre.Crime,
            5 => EGenre.Documentary,
            6 => EGenre.Drama,
            7 => EGenre.Fantasy,
            8 => EGenre.Horror,
            9 => EGenre.Musical,
            10 => EGenre.Mistery,
            11 => EGenre.Romance,
            12 => EGenre.SciFi,
            13 => EGenre.Thriller,
            14 => EGenre.Western,
            15 => EGenre.Biography,
            16 => EGenre.Historic,
            17 => EGenre.War,
            18 => EGenre.Family,
            _ => throw new NotImplementedException()
        };

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePublishedDate(DateTime date)
    {
        IsAvailable();
        var todayDate = DateTime.UtcNow;

        if (todayDate < date)
            throw new DomainException("Invalid published date");

        Published = date;
        UpdatedAt = todayDate;
    }

    public void UpdateDirector(Guid directorId)
    {
        IsAvailable();

        if (directorId == Guid.Empty)
            throw new DomainException("Invalid director's Id");

        DirectorId = directorId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCopies(int copies)
    {
        IsAvailable();

        if (copies < 0)
            throw new DomainException("Number of copies must be greater than zero.");

        Copies = copies;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DeleteDvd()
    {
        if (!Available)
            throw new DomainException($"Dvd {Title} is already deleted.");

        Available = false;
        Copies = 0;
        DeletedAt = DateTime.UtcNow;
    }
}
