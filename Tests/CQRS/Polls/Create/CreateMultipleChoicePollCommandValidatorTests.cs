using Application.Common.Behaviors;
using Application.CQRS.Polls.Commands.CreateCommand.Multiple;
using FluentValidation;
using MediatR;
using Moq;

namespace Tests.CQRS.Polls.Create;

public class CreateMultipleChoicePollCommandValidatorTests
{
    private readonly ValidationBehavior<CreateMultipleChoicePollCommand, Guid> _behavior;
    private readonly Mock<RequestHandlerDelegate<Guid>> _nextMock;
    private readonly List<IValidator<CreateMultipleChoicePollCommand>> _validators;

    public CreateMultipleChoicePollCommandValidatorTests()
    {
        _validators = new List<IValidator<CreateMultipleChoicePollCommand>>() { new CreateMultipleChoicePollCommandValidator() };
        _nextMock = new Mock<RequestHandlerDelegate<Guid>>();
        _behavior = new ValidationBehavior<CreateMultipleChoicePollCommand, Guid>(_validators);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenEmptyRequest()
    {
        // Arrange
        var command = new CreateMultipleChoicePollCommand();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _behavior.Handle(command, _nextMock.Object, CancellationToken.None));

        // Assert
        Assert.Equal(5, exception.Errors.Count());
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenInvalidRequest()
    {
        // Arrange
        var command = new CreateMultipleChoicePollCommand()
        {
            UserId = Guid.NewGuid(),
            Title = "THISTITLE",
            StartDate = DateTime.UtcNow,
            MaxSelections = -1, // <-- Invalid
            Options = new List<OptionDto>() { } // <-- Invalid
        };

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _behavior.Handle(command, _nextMock.Object, CancellationToken.None));

        // Assert
        Assert.Equal(2, exception.Errors.Count());
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenInvalidRequest2()
    {
        // Arrange
        var command = new CreateMultipleChoicePollCommand()
        {
            UserId = Guid.NewGuid(),
            Title = "THISTITLE",
            StartDate = DateTime.UtcNow,
            MaxSelections = 1,
            Options = new List<OptionDto>()
            {
                new OptionDto() { Fullname="123" } // <-- Invalid
            }
        };

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _behavior.Handle(command, _nextMock.Object, CancellationToken.None));

        // Assert
        Assert.Single(exception.Errors);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidRequest()
    {
        // Arrange
        var command = new CreateMultipleChoicePollCommand()
        {
            UserId = Guid.NewGuid(),
            Title = "THISTITLE",
            StartDate = DateTime.UtcNow,
            MaxSelections = 1,
            Options = new List<OptionDto>()
            {
                new OptionDto() { Fullname="Option" },
                new OptionDto() { Fullname="Option" },
                new OptionDto() { Fullname="Option" },
            }
        };
        _nextMock.Setup(x => x()).ReturnsAsync(Guid.Parse("aba44968-f62e-433c-9701-ee99fe37de80"));

        // Act
        var result = await _behavior.Handle(command, _nextMock.Object, CancellationToken.None);

        // Assert
        _nextMock.Verify(x => x(), Times.Once);
        Assert.Equal(Guid.Parse("aba44968-f62e-433c-9701-ee99fe37de80"), result);
    }
}
