using api_desafio.tech.Data;
using api_desafio.tech.DTOs;
using api_desafio.tech.Helpers;
using api_desafio.tech.Models;
using api_desafio.tech.Requests;
using Microsoft.AspNetCore.Identity;

namespace api_desafio.tech.EndPoints
{
    public static class UserEndPoint
    {
        public static void AddUserEndPoints(this WebApplication app)
        {
            var endpoint = app.MapGroup("users").RequireAuthorization();

            endpoint.MapGet("/{id:guid}", async (Guid id, AppDbContext context, CancellationToken ct) =>
            {
                var user = await context.Users.FindAsync(id, ct);
                if (user == null)
                {
                    return Results.NotFound();
                }

                var userDto = new UserDto(user.Id, user.Email, user.Roles);

                return Results.Ok(userDto);
            });

            endpoint.MapPut("edit/{id:guid}", async (Guid id, UpdateUserRequest request, AppDbContext context, IPasswordHasher<User> passwordHasher, CancellationToken ct) =>
            {
                var user = await context.Users.FindAsync(id, ct);
                if (user == null)
                {
                    return Results.NotFound();
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    if (!ValidationHelpers.IsValidEmail(request.Email))
                    {
                        return Results.BadRequest("Email inválido.");
                    }
                    user.UpdateEmail(request.Email);
                }

                if (!string.IsNullOrEmpty(request.NewPassword))
                {
                    if (!ValidationHelpers.IsValidPassword(request.NewPassword))
                    {
                        return Results.BadRequest("Senha inválida.");
                    }

                    if (string.IsNullOrEmpty(request.CurrentPassword))
                    {
                        return Results.BadRequest("Senha atual é obrigatória.");
                    }

                    var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, request.CurrentPassword);
                    if (passwordVerificationResult != PasswordVerificationResult.Success)
                    {
                        return Results.BadRequest("Senha atual incorreta.");
                    }

                    var hashedPassword = passwordHasher.HashPassword(user, request.NewPassword);
                    user.UpdatePassword(hashedPassword);
                }

                await context.SaveChangesAsync(ct);
                return Results.Ok();
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
