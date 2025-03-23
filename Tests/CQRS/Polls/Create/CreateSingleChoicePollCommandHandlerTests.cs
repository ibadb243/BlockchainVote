using Application.CQRS.Polls.Commands.CreateCommand.Single;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Persistence;

namespace Tests.CQRS.Polls.Create;

public class CreateSingleChoicePollCommandHandlerTests
{
    private readonly CreateSingleChoicePollCommandHandler _handler;
    private readonly ApplicationDbContext _context;

    public CreateSingleChoicePollCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "usercreate")
            .Options;

        _context = new ApplicationDbContext(options);

        _handler = new CreateSingleChoicePollCommandHandler(_context);
    }

    [Fact]
    public async Task Handle_ShouldCreateSingleChoicePoll_WhenValidRequest()
    {
        // Arrange
        await reset_database();
        var command = new CreateSingleChoicePollCommand()
        {
            UserId = Guid.Parse("fe560d8e-dbe4-4e5e-b1b2-da08b8145494"),
            Title = "Title123",
            Description = "Description",
            StartDate = DateTimeOffset.UtcNow,
            EndDate = DateTimeOffset.UtcNow.AddHours(1),
            IsAnonymous = false,
            Options = new List<OptionDto>()
            {
                new OptionDto() { Fullname="Fullname", Description="Description" },
                new OptionDto() { Fullname="Fullname", Description="Description" },
                new OptionDto() { Fullname="Fullname", Description="Description" },
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        var poll = await _context.SingleChoicePolls.FirstOrDefaultAsync(CancellationToken.None);
        Assert.NotNull(poll);
        Assert.Equal(Guid.Parse("fe560d8e-dbe4-4e5e-b1b2-da08b8145494"), poll.UserId);

        var options = await _context.PollOption.Where(x => x.PollId == poll.Id).ToListAsync();
        Assert.Equal(3, options.Count);
    }

    /// <summary>
    /// The request must be pass by validator before getted by handle
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCreateSingleChoicePoll_WhenInvalidRequest()
    {
        // Arrange
        await reset_database();
        var command = new CreateSingleChoicePollCommand()
        {
            Title = string.Empty,
            Options = new List<OptionDto>()
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        var poll = await _context.SingleChoicePolls.FirstOrDefaultAsync(CancellationToken.None);
        Assert.NotNull(poll);
        var options = await _context.PollOption.Where(x => x.PollId == poll.Id).ToListAsync();
        Assert.Empty(options);
    }

    /// <summary>
    /// The request must be pass by validator before getted by handle
    /// </summary>
    [Fact]
    public async Task Handle_ThrowDbContextExceptions_WhenEmptyRequest()
    {
        // Arrange
        await reset_database();
        var command = new CreateSingleChoicePollCommand();

        // Act
        var exception = await Assert.ThrowsAsync<DbUpdateException>(async () => await _handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.NotNull(exception);
        Assert.Contains("{'Title'}", exception.Message);
    }

    private async Task reset_database()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
    }
}
