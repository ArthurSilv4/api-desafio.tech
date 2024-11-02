namespace api_desafio.tech.Models.User
{
    public record User
    (
      int Id,
      string Email,
      string Password,
      string[] Roles
    );
}
