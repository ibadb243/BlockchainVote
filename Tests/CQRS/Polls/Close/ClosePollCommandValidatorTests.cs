using Application.Common.Behaviors;
using Application.CQRS.Polls.Commands.CloseCommand;
using FluentValidation;
using MediatR;
using Moq;

namespace Tests.CQRS.Polls.Close;

public class ClosePollCommandValidatorTests
{
    private readonly ValidationBehavior<ClosePollCommand, Unit> _behavior;
    private readonly Mock<RequestHandlerDelegate<Unit>> _nextMock;
    private readonly List<IValidator<ClosePollCommand>> _validators;

    public ClosePollCommandValidatorTests()
    {
        _validators = new List<IValidator<ClosePollCommand>>() { new ClosePollCommandValidator() };
        _nextMock = new Mock<RequestHandlerDelegate<Unit>>();
        _behavior = new ValidationBehavior<ClosePollCommand, Unit>(_validators);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenInvalidRequest2()
    {
        // Arrange
        var command = new ClosePollCommand();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _behavior.Handle(command, _nextMock.Object, CancellationToken.None));

        // Assert
        Assert.Equal(2, exception.Errors.Count());
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidRequest()
    {
        // Arrange
        var command = new ClosePollCommand()
        {
            UserId = Guid.NewGuid(),
            PollId = Guid.NewGuid(),
            
        };
        _nextMock.Setup(x => x()).ReturnsAsync(Unit.Value);

        // Act
        var result = await _behavior.Handle(command, _nextMock.Object, CancellationToken.None);

        // Assert
        _nextMock.Verify(x => x(), Times.Once);
        Assert.Equal(Unit.Value, result);
    }
}
