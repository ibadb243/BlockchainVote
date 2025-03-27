using MediatR;

namespace Application.CQRS.SinglePolls.Commands.CreateCommand;

public class CreateSingleChoicePollCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsAnonymous { get; set; } = false;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public List<OptionDto>  Options { get; set; } = new List<OptionDto>();
}
