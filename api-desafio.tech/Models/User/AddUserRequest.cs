namespace api_desafio.tech.Models.User
{
    public record AddUserRequest(string Email, string Password, string[] Roles);

}
