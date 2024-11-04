namespace api_desafio.tech.Models.Auth
{
    public record RegisterRequest(string Email, string Password, string[] Roles);

}
