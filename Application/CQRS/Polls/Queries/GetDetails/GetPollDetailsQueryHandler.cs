using Application.Common.Exceptions;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Entities.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.CQRS.Polls.Queries.GetDetails;

public class GetPollDetailsQueryHandler : IRequestHandler<GetPollDetailsQuery, PollVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPollDetailsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PollVm> Handle(GetPollDetailsQuery request, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == request.PollId, cancellationToken);
        if (poll == null) throw new NotFoundException<PollBase>();

        var vm = _mapper.Map<PollVm>(poll);

        vm.Options = await _context.PollOption
            .Where(o => o.PollId == request.PollId)
            .ProjectTo<OptionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        if (!vm.IsClosed) return vm;

        var optionCount = vm.Options.Count();

        switch (poll)
        {
            case SingleChoicePoll single:
                vm.Results = await _context.SingleVotes
                    .Where(v => v.PollId == request.PollId)
                    .GroupBy(v => v.OptionId)
                    .Select(v => new 
                    {
                        OptionId = v.Key,
                        VotesCount = v.Count()
                    })
                    .ToDictionaryAsync(g => g.OptionId, g => g.VotesCount, cancellationToken);
                break;
            case MultipleChoicePoll multiple:
                vm.Results = await _context.MultipleVotes
                    .Where(v => v.PollId == request.PollId)
                    .SelectMany(v => _getVotes(v.Options))
                    .GroupBy(kv => kv.Key)
                    .ToDictionaryAsync(g => g.Key, g => g.Sum(kv => kv.Value));
                break;
            case QuickPoll quick:
                vm.Results = await _context.QuickVotes
                    .Where(v => v.PollId == request.PollId)
                    .GroupBy(o => o.OptionId)
                    .Select(o => new
                    {
                        OptionId = o.Key,
                        VotesCount = o.Count()
                    })
                    .ToDictionaryAsync(o => o.OptionId, o => o.VotesCount, cancellationToken);
                break;
        }

        return vm;
    }

    private Dictionary<int, int> _getVotes(int votes)
    {
        var result = new Dictionary<int, int>();

        for (int i = 0; votes != 0; i++, votes >>= 1)
        {
            if ((votes & 1) == 1) result[i] = 1;
        }

        return result;
    }
}
