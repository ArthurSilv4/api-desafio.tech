namespace api_desafio.tech.Models.Challenges
{
    public record ChallengeDto(Guid Id, string? Title, string? Description, DateTime StartDate, DateTime EndDate, List<DateTime> ChallengeDates, bool Completed, Guid? UserId);
}
