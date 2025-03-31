using Application.Common.Exceptions;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Entities.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.CQRS.QuickPolls.Queries.GetDetails;

public class GetQuickPollDetailsQueryHandler : IRequestHandler<GetQuickPollDetailsQuery, QuickPollVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetQuickPollDetailsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<QuickPollVm> Handle(GetQuickPollDetailsQuery request, CancellationToken cancellationToken)
    {
        var poll = await _context.QuickPolls.FirstOrDefaultAsync(p => p.Id == request.PollId, cancellationToken);
        if (poll == null) throw new NotFoundException<PollBase>();

        var vm = _mapper.Map<QuickPollVm>(poll);

        vm.Options = await _context.PollOption
            .Where(o => o.PollId == request.PollId)
            .ProjectTo<OptionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        if (!vm.IsClosed) return vm;

        var optionCount = vm.Options.Count();

        vm.Results = await _context.QuickVotes
            .Where(v => v.PollId == request.PollId)
            .GroupBy(o => o.OptionId)
            .Select(o => new
            {
                OptionId = o.Key,
                VotesCount = o.Count()
            })
            .ToDictionaryAsync(o => o.OptionId, o => o.VotesCount, cancellationToken);

        return vm;
    }
}
