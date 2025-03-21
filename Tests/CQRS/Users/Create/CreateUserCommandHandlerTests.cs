using Application.CQRS.Users.Commands.CreateCommand;
using Application.Interfaces;
using Persistence;
using Moq;
using Microsoft.EntityFrameworkCore;
using Application.Common.Exceptions;
using Domain.Entities;

namespace Tests.CQRS.Users.Create;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IHashHelper> _hashHelperMock;
    private readonly CreateUserCommandHandler _handler;
    private readonly ApplicationDbContext _context;

    public CreateUserCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "usercreate")
            .Options;

        _context = new ApplicationDbContext(options);

        _hashHelperMock = new Mock<IHashHelper>();
        _hashHelperMock.Setup(x => x.CalculateHash(It.IsAny<string>())).Returns("hash");

        _handler = new CreateUserCommandHandler(_context, _hashHelperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUserCommand_WhenValidRequest()
    {
        // Arrange
        var command = new CreateUserCommand { FIN = "ABCDEFG", Password = "PASSWORD" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        var user = await _context.Users.FirstOrDefaultAsync();

        Assert.Equal(result, user.Id);
        Assert.Equal("hash", user.FINHash);
        Assert.Equal("hash", user.FINHash);
    }

    [Fact]
    public async Task Handle_ShouldCreateUserCommand_WhenFinIsAlreadyUsed()
    {
        // Arrange
        var command = new CreateUserCommand { FIN = "ABCDEFG", Password = "PASSWORD" };
        await _context.Users.AddAsync(new User("hash", "hash"));
        await _context.SaveChangesAsync();

        // Act
        var exception = await Assert.ThrowsAsync<FinIsAlreadyUsedException>(() => _handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal("FIN ABCDEFG is already used!", exception.Message);
    }
}
