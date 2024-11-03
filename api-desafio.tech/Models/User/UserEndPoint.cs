using api_desafio.tech.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api_desafio.tech.Models.User
{
    public static class UserEndPoint
    {
        public static void AddUserEndPoints(this WebApplication app)
        {
            var endpoint = app.MapGroup("users");

            endpoint.MapPost("/create", async (AddUserRequest request, AppDbContext context, CancellationToken ct) =>
            {
                var userExists = await context.Users.AnyAsync(user => user.Email == request.Email, ct);

                if (userExists)
                {
                    return Results.Conflict("Ja existe!");
                }

                var passwordHasher = new PasswordHasher<User>();
                var newUser = new User(request.Email, request.Password, request.Roles);
                var hashedPassword = passwordHasher.HashPassword(newUser, request.Password);
                newUser.SetHashedPassword(hashedPassword);

                await context.Users.AddAsync(newUser, ct);
                await context.SaveChangesAsync(ct);

                var userReturn = new UserDto(newUser.Id, newUser.Email, newUser.Roles);
                return Results.Ok(userReturn);
            });

            endpoint.MapDelete("delete/{id:guid}", async (Guid id, AppDbContext context, CancellationToken ct) =>
            {
                var user = await context.Users.FindAsync(id, ct);
                if (user == null)
                {
                    return Results.NotFound();
                }

                user.DisabeUser();

                await context.SaveChangesAsync(ct);
                return Results.Ok();
            });
        }
    }
}
