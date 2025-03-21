using Application.Common.Behaviors;
using Application.CQRS.Users.Commands.CreateCommand;
using FluentValidation;
using MediatR;
using Moq;

namespace Tests.CQRS.Users.Create;

public class CreateUserCommandValidatorTests
{
    private readonly ValidationBehavior<CreateUserCommand, Guid> _behavior;
    private readonly Mock<RequestHandlerDelegate<Guid>> _nextMock;
    private readonly List<IValidator<CreateUserCommand>> _validators;

    public CreateUserCommandValidatorTests()
    {
        _validators = new List<IValidator<CreateUserCommand>>() { new CreateUserCommandValidator() };
        _nextMock = new Mock<RequestHandlerDelegate<Guid>>();
        _behavior = new ValidationBehavior<CreateUserCommand, Guid>(_validators);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenInvalidRequest()
    {
        // Arrange
        var command = new CreateUserCommand { FIN = "", Password = "" };

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _behavior.Handle(command, _nextMock.Object, CancellationToken.None));

        // Assert
        Assert.Equal(2, exception.Errors.Count());
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidRequest()
    {
        // Arrange
        var command = new CreateUserCommand { FIN = "ABCDEFG", Password = "PASSWORD" };
        _nextMock.Setup(x => x()).ReturnsAsync(Guid.Parse("aba44968-f62e-433c-9701-ee99fe37de80"));

        // Act
        var result = await _behavior.Handle(command, _nextMock.Object, CancellationToken.None);

        // Assert
        _nextMock.Verify(x => x(), Times.Once);
        Assert.Equal(Guid.Parse("aba44968-f62e-433c-9701-ee99fe37de80"), result);
    }
}
