using api_desafio.tech.Data;
using api_desafio.tech.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using api_desafio.tech.Requests;
using api_desafio.tech.Models;
using api_desafio.tech.DTOs;

namespace api_desafio.tech.EndPoints
{
    public static class AuthEndPoint
    {
        public static void AuthEndPoints(this WebApplication app)
        {
            var endpoint = app.MapGroup("auth");

            endpoint.MapPost("/login", async (LoginRequest request, AppDbContext context, TokenService tokenService, IPasswordHasher<User> passwordHasher) =>
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null || passwordHasher.VerifyHashedPassword(user, user.Password, request.Password) == PasswordVerificationResult.Failed)
                {
                    return Results.Unauthorized();
                }

                var token = tokenService.Generate(user);
                return Results.Ok(new { Token = token });
            });

            endpoint.MapPost("/register", async (RegisterRequest request, AppDbContext context, CancellationToken ct) =>
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
        }
    }
}
