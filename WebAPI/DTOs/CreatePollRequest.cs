namespace WebAPI.DTOs
{
    public record CreatePollRequest(
        string Title,
        List<CreateCandidateDto> Candidates,
        DateTime EndTime,
        DateTime? StartTime = null,
        bool IsSurvey = false,
        bool AllowRevote = false,
        int? MaxSelections = null,
        bool IsAnonymous = false
    );
}
