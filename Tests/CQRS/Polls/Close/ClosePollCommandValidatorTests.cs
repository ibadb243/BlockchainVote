using Application.Common.Behaviors;
using Application.CQRS.Polls.Commands.CloseCommand;
using FluentValidation;
using MediatR;
using Moq;

namespace Tests.CQRS.Polls.Close;

public class ClosePollCommandValidatorTests
{
    private readonly ValidationBehavior<CloseSingleChoicePollCommand, Unit> _behavior;
    private readonly Mock<RequestHandlerDelegate<Unit>> _nextMock;
    private readonly List<IValidator<CloseSingleChoicePollCommand>> _validators;

    public ClosePollCommandValidatorTests()
    {
        _validators = new List<IValidator<CloseSingleChoicePollCommand>>() { new CloseSingleChoicePollCommandValidator() };
        _nextMock = new Mock<RequestHandlerDelegate<Unit>>();
        _behavior = new ValidationBehavior<CloseSingleChoicePollCommand, Unit>(_validators);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenInvalidRequest2()
    {
        // Arrange
        var command = new CloseSingleChoicePollCommand();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _behavior.Handle(command, _nextMock.Object, CancellationToken.None));

        // Assert
        Assert.Equal(2, exception.Errors.Count());
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidRequest()
    {
        // Arrange
        var command = new CloseSingleChoicePollCommand()
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
