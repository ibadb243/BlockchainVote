using Application.Common.Exceptions;
using Application.CQRS.Polls.Commands.CloseCommand;
using Domain.Entities;
using Domain.Entities.Abstract;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Tests.CQRS.Polls.Close;

public class ClosePollCommandHandlerTests
{
    private readonly CloseSingleChoicePollCommandHandler _handler;
    private readonly ApplicationDbContext _context;

    public ClosePollCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "pollclose")
            .Options;

        _context = new ApplicationDbContext(options);

        _handler = new CloseSingleChoicePollCommandHandler(_context);
    }

    [Fact]
    public async Task Handle_ShouldClosePoll_WhenValidRequest()
    {
        // Arrange
        await reset_database();
        var poll = new SingleChoicePoll
        {
            Id = Guid.Parse("35e9803c-2af0-4a08-bafd-af3827b4e0d7"),
            UserId = Guid.Parse("fee6c730-0e98-4dc8-8f70-ca009beea506"),
            Title = "Title"
        };
        await _context.SingleChoicePolls.AddAsync(poll);
        await _context.SaveChangesAsync();
        var command = new CloseSingleChoicePollCommand
        {
            UserId = poll.UserId,
            PollId = poll.Id,
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var singleP = await _context.SingleChoicePolls.FirstOrDefaultAsync(p => p.Id == poll.Id);
        Assert.NotNull(singleP);
        Assert.NotNull(singleP.EndDate);
    }

    [Fact]
    public async Task Handle_ShouldCloseUnclosedPoll_WhenValidRequest()
    {
        // Arrange
        await reset_database();
        var poll = new SingleChoicePoll
        {
            Id = Guid.Parse("35e9803c-2af0-4a08-bafd-af3827b4e0d7"),
            UserId = Guid.Parse("fee6c730-0e98-4dc8-8f70-ca009beea506"),
            Title = "Title",
            EndDate = DateTime.Now.AddDays(5),
        };
        await _context.SingleChoicePolls.AddAsync(poll);
        await _context.SaveChangesAsync();
        var command = new CloseSingleChoicePollCommand
        {
            UserId = poll.UserId,
            PollId = poll.Id,
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var singleP = await _context.SingleChoicePolls.FirstOrDefaultAsync(p => p.Id == poll.Id);
        Assert.NotNull(singleP);
        Assert.NotNull(singleP.EndDate);
    }

    [Fact]
    public async Task Handle_ShouldThrowPollClosedException_WhenValidRequest()
    {
        // Arrange
        await reset_database();
        var poll = new SingleChoicePoll
        {
            Id = Guid.Parse("35e9803c-2af0-4a08-bafd-af3827b4e0d7"),
            UserId = Guid.Parse("fee6c730-0e98-4dc8-8f70-ca009beea506"),
            Title = "Title",
            EndDate = DateTime.Now.AddMinutes(-5),
        };
        await _context.SingleChoicePolls.AddAsync(poll);
        await _context.SaveChangesAsync();
        var command = new CloseSingleChoicePollCommand
        {
            UserId = poll.UserId,
            PollId = poll.Id,
        };

        // Act
        var exception = await Assert.ThrowsAsync<PollClosedException>(async () => await _handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("The Poll is already closed!", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenUsersIdNotMatch()
    {
        // Arrange
        await reset_database();
        var poll = new SingleChoicePoll
        {
            Id = Guid.Parse("35e9803c-2af0-4a08-bafd-af3827b4e0d7"),
            UserId = Guid.Parse("fee6c730-0e98-4dc8-8f70-ca009beea506"),
            Title = "Title",
            EndDate = DateTime.Now.AddMinutes(-5),
        };
        await _context.SingleChoicePolls.AddAsync(poll);
        await _context.SaveChangesAsync();
        var command = new CloseSingleChoicePollCommand
        {
            UserId = Guid.NewGuid(),
            PollId = poll.Id,
        };

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException<PollBase>>(async () => await _handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenPollsIdNotMatch()
    {
        // Arrange
        await reset_database();
        var poll = new SingleChoicePoll
        {
            Id = Guid.Parse("35e9803c-2af0-4a08-bafd-af3827b4e0d7"),
            UserId = Guid.Parse("fee6c730-0e98-4dc8-8f70-ca009beea506"),
            Title = "Title",
            EndDate = DateTime.Now.AddMinutes(-5),
        };
        await _context.SingleChoicePolls.AddAsync(poll);
        await _context.SaveChangesAsync();
        var command = new CloseSingleChoicePollCommand
        {
            UserId = poll.UserId,
            PollId = Guid.NewGuid(),
        };

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException<PollBase>>(async () => await _handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.NotNull(exception);
    }

    private async Task reset_database()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
    }
}
