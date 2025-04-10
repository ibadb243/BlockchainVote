using Application.Common.Mappings;
using Application.CQRS.SinglePolls.Queries.GetDetails;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;
using System.Reflection;

namespace Tests.CQRS.Polls.GetDetails;

public class GetSinglePollDetailsQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly GetSinglePollDetailsQueryHandler _handler;
    private readonly ApplicationDbContext _context;

    public GetSinglePollDetailsQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "getsinglepolldetails")
            .Options;

        _context = new ApplicationDbContext(options);

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AssemblyMappingProfile(typeof(SinglePollVm).Assembly));
        });

        _mapper = configuration.CreateMapper();

        _handler = new GetSinglePollDetailsQueryHandler(_context, _mapper);
    }

    [Fact]
    public async Task Handler_ShouldReturnUnclosedSingleChoicePoll_WhenPollIsNotClosed()
    {
        // Arrange
        await reset_database();
        var rand = new Random();
        var poll = new SingleChoicePoll(Guid.NewGuid(), "TITLE", "DESC", DateTimeOffset.MinValue);
        var options = await create_polloptions(poll.Id, 3);
        var votes = await create_singlevotes(poll.Id, 3, 15);
        await _context.SingleChoicePolls.AddAsync(poll);
        await _context.PollOption.AddRangeAsync(options);
        await _context.AddRangeAsync(votes);
        await _context.SaveChangesAsync();
        var query = new GetSinglePollDetailsQuery() { PollId = poll.Id };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(options.Count, result.Options.Count);
        Assert.False(result.IsClosed);
        Assert.Null(result.Results);
    }

    [Fact]
    public async Task Handler_ShouldReturnClosedSingleChoicePoll_WhenPollIsClosed()
    {
        // Arrange
        await reset_database();
        var rand = new Random();
        var poll = new SingleChoicePoll(Guid.NewGuid(), "TITLE", "DESC", DateTimeOffset.MinValue, DateTimeOffset.MinValue.AddHours(2));
        var optionCount = 3;
        var options = await create_polloptions(poll.Id, optionCount);
        var voteCount = 15;
        var votes = await create_singlevotes(poll.Id, optionCount, voteCount);
        await _context.SingleChoicePolls.AddAsync(poll);
        await _context.PollOption.AddRangeAsync(options);
        await _context.AddRangeAsync(votes);
        await _context.SaveChangesAsync();
        var query = new GetSinglePollDetailsQuery() { PollId = poll.Id };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(optionCount, result.Options.Count);
        Assert.True(result.IsClosed);
        Assert.NotNull(result.Results);
        Assert.Equal(voteCount, result.Results.Count);
    }

    private async Task reset_database()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
    }
    private async Task<List<PollOption>> create_polloptions(Guid pollId, int count)
    {
        var result = new List<PollOption>();

        for (int i = 0; i < count % 32; i++)
        {
            result.Add(new PollOption(pollId, (int)Math.Pow(2, i), $"FULLNAME::{i}"));
        }

        return result;
    }
    private async Task<List<SingleVote>> create_singlevotes(Guid pollId, int optionCount, int count)
    {
        var result = new List<SingleVote>();
        var rnd = new Random();

        for (int i = 0; i < count; i++)
        {
            result.Add(new SingleVote(pollId, Guid.NewGuid(), (int)Math.Pow(2, rnd.Next(0, optionCount))));
        }

        return result;
    }
}
