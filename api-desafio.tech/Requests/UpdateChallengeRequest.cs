namespace api_desafio.tech.Requests
{
    public record UpdateChallengeRequest(string Title, string Description, DateTime StartDate, bool Completed);
}
