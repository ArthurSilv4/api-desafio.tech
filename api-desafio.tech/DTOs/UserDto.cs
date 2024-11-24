namespace api_desafio.tech.DTOs
{
    public record UserDto(Guid Id, string Name, string? Description, string Email, string[] Roles, int Xp, int Level, string[]? SocialMedia);
}
