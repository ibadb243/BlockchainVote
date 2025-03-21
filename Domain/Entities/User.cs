namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FINHash { get; set; }
    public string PasswordHash { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public User() { }

    public User(
        string finHash,
        string passwordHash)
    {
        Id = Guid.NewGuid();
        FINHash = finHash;
        PasswordHash = passwordHash;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}