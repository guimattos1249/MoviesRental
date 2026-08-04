namespace MoviesRental.Core.DomainObjects;

public class Entity
{
    public Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;
    }
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
}
