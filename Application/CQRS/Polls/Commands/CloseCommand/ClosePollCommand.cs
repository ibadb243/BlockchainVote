﻿using MediatR;

namespace Application.CQRS.Polls.Commands.CloseCommand;

public class CloseSingleChoicePollCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid PollId { get; set; }
}
