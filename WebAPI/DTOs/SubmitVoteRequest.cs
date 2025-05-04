namespace WebAPI.DTOs
{
    public record SubmitVoteRequest(Guid PollId, List<int> CandidateIds);
}
