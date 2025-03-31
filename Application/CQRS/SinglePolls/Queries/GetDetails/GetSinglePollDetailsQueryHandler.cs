using Application.Common.Exceptions;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Entities.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.CQRS.SinglePolls.Queries.GetDetails;

public class GetSinglePollDetailsQueryHandler : IRequestHandler<GetSinglePollDetailsQuery, SinglePollVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSinglePollDetailsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SinglePollVm> Handle(GetSinglePollDetailsQuery request, CancellationToken cancellationToken)
    {
        var poll = await _context.SingleChoicePolls.FirstOrDefaultAsync(p => p.Id == request.PollId, cancellationToken);
        if (poll == null) throw new NotFoundException<PollBase>();

        var vm = _mapper.Map<SinglePollVm>(poll);

        vm.Options = await _context.PollOption
            .Where(o => o.PollId == request.PollId)
            .ProjectTo<OptionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        if (!vm.IsClosed) return vm;

        vm.Results = await _context.SingleVotes
            .Where(v => v.PollId == request.PollId)
            .GroupBy(v => v.OptionId)
            .Select(v => new
            {
                OptionId = v.Key,
                VotesCount = v.Count()
            })
            .ToDictionaryAsync(g => g.OptionId, g => g.VotesCount, cancellationToken);

        return vm;
    }
}
