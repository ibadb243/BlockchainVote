using Application.Common.Exceptions;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Entities.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.CQRS.MultiplePolls.Queries.GetDetails;

public class GetMultiplePollDetailsQueryHandler : IRequestHandler<GetMultiplePollDetailsQuery, MultiplePollVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMultiplePollDetailsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<MultiplePollVm> Handle(GetMultiplePollDetailsQuery request, CancellationToken cancellationToken)
    {
        var poll = await _context.MultipleChoicePolls.FirstOrDefaultAsync(p => p.Id == request.PollId, cancellationToken);
        if (poll == null) throw new NotFoundException<PollBase>();

        var vm = _mapper.Map<MultiplePollVm>(poll);

        vm.Options = await _context.PollOption
            .Where(o => o.PollId == request.PollId)
            .ProjectTo<OptionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        if (!vm.IsClosed) return vm;

        var optionCount = vm.Options.Count();

        vm.Results = await _context.MultipleVotes
            .Where(v => v.PollId == request.PollId)
            .SelectMany(v => _getVotes(v.Options))
            .GroupBy(kv => kv.Key)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(kv => kv.Value));

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
