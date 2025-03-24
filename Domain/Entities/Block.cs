namespace Domain.Entities;

public class Block
{
    public int Id { get; set; }
    public string MerkleRoot { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string PreviousHash { get; set; }
    public string Hash { get; set; }
    public int Nonce { get; set; }

    public Block() { }

    public Block(
        int id,
        string merkleRoot,
        DateTimeOffset timestamp,
        string previousHash,
        string hash,
        int nonce)
    {
        Id = id;
        MerkleRoot = merkleRoot;
        Timestamp = timestamp;
        PreviousHash = previousHash;
        Hash = hash;
        Nonce = nonce;
    }
}