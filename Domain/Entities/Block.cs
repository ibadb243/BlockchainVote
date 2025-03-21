namespace Domain.Entities;

public class Block
{
    public int Id { get; set; }
    public string VotesJson { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string PreviousHash { get; set; }
    public string Hash { get; set; }
    public int Nonce { get; set; }

    public Block(
        int id,
        string votesJson,
        DateTimeOffset timestamp,
        string previousHash,
        string hash,
        int nonce)
    {
        Id = id;
        VotesJson = votesJson;
        Timestamp = timestamp;
        PreviousHash = previousHash;
        Hash = hash;
        Nonce = nonce;
    }
}