namespace api_desafio.tech.Requests
{
    public record UpdateUserRequest(string? Email, string? CurrentPassword, string? NewPassword);
}
