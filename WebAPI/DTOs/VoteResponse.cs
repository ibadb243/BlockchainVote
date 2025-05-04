namespace WebAPI.DTOs
{
    public record VoteResponse(Guid Id, Guid PollId, Guid UserId, List<int> CandidateIds, DateTime SubmittedAt);
}
