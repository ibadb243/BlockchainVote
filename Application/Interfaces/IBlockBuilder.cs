using Domain.Entities;
using Domain.Entities.Abstract;

namespace Application;

public interface IBlockBuilder
{
    IBlockBuilder Reset();
    IBlockBuilder SetId(int id);
    IBlockBuilder SetTimestamp(DateTimeOffset timestamp);
    IBlockBuilder AddVote(VoteBase vote);
    IBlockBuilder AddVotes(IEnumerable<VoteBase> votes);
    IBlockBuilder AddPreviousHash(string previousHash);

    Block Build(int difficulty);
}
