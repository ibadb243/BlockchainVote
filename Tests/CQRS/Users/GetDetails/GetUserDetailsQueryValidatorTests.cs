using Application.Common.Behaviors;
using Application.CQRS.Users.Queries.GetDetailsQuery;
using FluentValidation;
using MediatR;
using Moq;

namespace Tests.CQRS.Users.GetDetails;

public class GetUserDetailsQueryValidatorTests
{
    private readonly ValidationBehavior<GetUserDetailsQuery, UserVm> _behavior;
    private readonly Mock<RequestHandlerDelegate<UserVm>> _nextMock;
    private readonly List<IValidator<GetUserDetailsQuery>> _validators;

    public GetUserDetailsQueryValidatorTests()
    {
        _validators = new List<IValidator<GetUserDetailsQuery>>() { new GetUserDetailsQueryValidator() };
        _nextMock = new Mock<RequestHandlerDelegate<UserVm>>();
        _behavior = new ValidationBehavior<GetUserDetailsQuery, UserVm>(_validators);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenInvalidRequest()
    {
        // Arrange
        var query = new GetUserDetailsQuery { UserId = Guid.Empty };

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _behavior.Handle(query, _nextMock.Object, CancellationToken.None));

        // Assert
        Assert.Single(exception.Errors);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidRequest()
    {
        // Arrange
        var query = new GetUserDetailsQuery { UserId = Guid.NewGuid() };
        _nextMock.Setup(x => x()).ReturnsAsync(new UserVm() { CreatedAt = DateTimeOffset.MaxValue });

        // Act
        var result = await _behavior.Handle(query, _nextMock.Object, CancellationToken.None);

        // Assert
        _nextMock.Verify(x => x(), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(DateTimeOffset.MaxValue, result.CreatedAt);
    }
}
