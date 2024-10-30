namespace api_desafio.tech.Models.Challenges
{
    public record UpdateChallengeRequest(string Title, string Description, DateTime StartDate, bool Completed);
}
