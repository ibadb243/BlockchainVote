using Application.Common.Exceptions;
using Application.CQRS.Users.Queries.GetDetailsQuery;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;

namespace Tests.CQRS.Users.GetDetails;

public class GetUserDetailsQueryHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserDetailsQueryHandler _handler;
    private readonly ApplicationDbContext _context;

    public GetUserDetailsQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "userget")
            .Options;

        _context = new ApplicationDbContext(options);

        _mapperMock = new Mock<IMapper>();
        _mapperMock.Setup(x => x.Map<UserVm>(It.IsAny<User>())).Returns(new UserVm() { CreatedAt = DateTimeOffset.MaxValue });

        _handler = new GetUserDetailsQueryHandler(_context, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserVm_WhenValidRequest()
    {
        // Arrange
        var user = new User("hash", "hash");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var query = new GetUserDetailsQuery { UserId = user.Id };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(result.CreatedAt, DateTimeOffset.MaxValue);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var query = new GetUserDetailsQuery { UserId = Guid.Empty };

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException<User>>(() => _handler.Handle(query, CancellationToken.None));

        // Assert
        Assert.Equal("User not found!", exception.Message);
    }
}
