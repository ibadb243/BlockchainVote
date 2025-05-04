namespace WebAPI.DTOs
{
    public record PollResponse(
        Guid Id,
        string Title,
        Guid UserId,
        List<CandidateResponse> Candidates,
        DateTime StartTime,
        DateTime EndTime,
        bool IsSurvey,
        bool AllowRevote,
        int? MaxSelections,
        bool IsAnonymous,
        DateTime CreatedAt
    );
}
