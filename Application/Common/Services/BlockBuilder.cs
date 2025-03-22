using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Abstract;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Application.Common.Services;

public class BlockBuilder : IBlockBuilder
{
    private readonly IHashHelper _hashHelper;

    private int _id;
    private List<VoteBase> _votes;
    private DateTimeOffset _timestamp;
    private string _previousHash;

    public BlockBuilder(IHashHelper hashHelper)
    {
        _hashHelper = hashHelper;

        _id = 0;
        _votes = new List<VoteBase>();
        _timestamp = DateTimeOffset.UtcNow;
        _previousHash = string.Empty;
    }

    public IBlockBuilder Reset()
    {
        _id = 0;
        _votes = new List<VoteBase>();
        _timestamp = DateTimeOffset.UtcNow;
        _previousHash = string.Empty;
        return this;
    }

    public IBlockBuilder SetId(int id)
    {
        _id = id;
        return this;
    }

    public IBlockBuilder SetTimestamp(DateTimeOffset timestamp)
    {
        _timestamp = timestamp;
        return this;
    }

    public IBlockBuilder AddVote(VoteBase vote)
    {
        _votes.Add(vote);
        return this;
    }

    public IBlockBuilder AddVotes(IEnumerable<VoteBase> votes)
    {
        _votes.AddRange(votes);
        return this;
    }

    public IBlockBuilder AddPreviousHash(string previousHash)
    {
        _previousHash = previousHash;
        return this;
    }

    public Block Build(int difficulty)
    {
        string target = new('0', difficulty);

        int nonce = -1;
        string votesJson = JsonSerializer.Serialize(_votes);

        string hash = string.Empty;
        do
        {
            hash = _hashHelper.CalculateHash($"{_id}{votesJson}{_timestamp}{_previousHash}{++nonce}");
        } while (!hash.StartsWith(target));

        return new Block(_id, votesJson, _timestamp, _previousHash, hash, nonce);
    }
}
