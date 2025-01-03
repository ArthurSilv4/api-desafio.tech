namespace api_desafio.tech.DTOs
{
    public record ChallengeDto(Guid Id, string author, string? Title, string? Description, DateTime StartDate, DateTime EndDate, List<ChallengeDay> ChallengeDays, bool Completed, Guid? UserId, string? UserName, string Status);
}
