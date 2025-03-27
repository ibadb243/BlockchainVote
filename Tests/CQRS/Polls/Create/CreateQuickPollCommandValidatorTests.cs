using Application.Common.Behaviors;
using Application.CQRS.QuickPolls.Commands.CreateCommand;
using FluentValidation;
using MediatR;
using Moq;

namespace Tests.CQRS.Polls.Create;

public class CreateQuickPollCommandValidatorTests
{
    private readonly ValidationBehavior<CreateQuickPollCommand, Guid> _behavior;
    private readonly Mock<RequestHandlerDelegate<Guid>> _nextMock;
    private readonly List<IValidator<CreateQuickPollCommand>> _validators;

    public CreateQuickPollCommandValidatorTests()
    {
        _validators = new List<IValidator<CreateQuickPollCommand>>() { new CreateQuickPollCommandValidator() };
        _nextMock = new Mock<RequestHandlerDelegate<Guid>>();
        _behavior = new ValidationBehavior<CreateQuickPollCommand, Guid>(_validators);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenEmptyRequest()
    {
        // Arrange
        var command = new CreateQuickPollCommand();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _behavior.Handle(command, _nextMock.Object, CancellationToken.None));

        // Assert
        Assert.Equal(4, exception.Errors.Count());
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenInvaliddRequest()
    {
        // Arrange
        var command = new CreateQuickPollCommand()
        {
            UserId = Guid.NewGuid(),
            Title = "THISTITLE",
            StartDate = DateTime.UtcNow,
            Options = new List<OptionDto>() { } // <-- Invalid
        };

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _behavior.Handle(command, _nextMock.Object, CancellationToken.None));

        // Assert
        Assert.Single(exception.Errors);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenInvaliddRequest2()
    {
        // Arrange
        var command = new CreateQuickPollCommand()
        {
            UserId = Guid.NewGuid(),
            Title = "THISTITLE",
            StartDate = DateTime.UtcNow,
            Duration = TimeSpan.FromMinutes(1), // <--Invalid
            Options = new List<OptionDto>()
            {
                new OptionDto() { Fullname="123456789" }
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
        var command = new CreateQuickPollCommand()
        {
            UserId = Guid.NewGuid(),
            Title = "THISTITLE",
            StartDate = DateTime.UtcNow,
            Duration = TimeSpan.FromMinutes(7),
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
