using MoviesRental.Core.DomainObjects;
using System.Text.RegularExpressions;

namespace MoviesRental.Domain.Entities;

public class Director : Entity
{
    public Director(string name, string surname)
    {
        UpdateName(name);
        UpdateSurname(surname);
    }

    const int MaxNameLength = 100;
    const int MinNameLength = 3;

    public string Name { get; private set; }
    public string Surname { get; private set; }
    private List<Dvd> _dvds = [];
    public IReadOnlyList<Dvd> Dvds => _dvds.AsReadOnly();

    public string FullName => $"{Name} {Surname}";

    public void UpdateName(string name)
    {
        if (ValidateName(name))
            throw new DomainException("Director name cannot be empty.");
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSurname(string surname)
    {
        if (ValidateName(surname))
            throw new DomainException("Director surname cannot be empty.");
        Surname = surname;
        UpdatedAt = DateTime.UtcNow;
    }

    private static bool ValidateName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name) || name.Length < MinNameLength || name.Length > MaxNameLength)
            return false;

        return Regex.IsMatch(name, "^(?=.*[A-ZÀ-ÿ~])(?=.*[a-zà-ÿ~])[A-Za-zÀ-ÿ~]+$");
    }
}
