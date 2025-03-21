using Domain.Entities;

namespace Application;

public interface IBlockBuilder
{
    IBlockBuilder Reset();
    IBlockBuilder SetId(int id);
    IBlockBuilder SetTimestamp(DateTimeOffset timestamp);
    IBlockBuilder AddVote(Vote vote);
    IBlockBuilder AddVotes(IEnumerable<Vote> votes);
    IBlockBuilder AddPreviousHash(string previousHash);

    Block Build(int difficulty);
}
