using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetVote
{
    public class GetVoteQueryHandler : IRequestHandler<GetVoteQuery, VoteVerificationDto?>
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IMapper _mapper;

        public GetVoteQueryHandler(IVoteRepository voteRepository, IMapper mapper)
        {
            _voteRepository = voteRepository;
            _mapper = mapper;
        }

        public async Task<VoteVerificationDto?> Handle(GetVoteQuery request, CancellationToken cancellationToken)
        {
            var vote = await _voteRepository.GetByIdAsync(request.Id);
            return vote == null ? null : _mapper.Map<VoteVerificationDto>(vote);
        }
    }
}
