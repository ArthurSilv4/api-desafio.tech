using api_desafio.tech.Data;
using api_desafio.tech.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using api_desafio.tech.Requests;
using api_desafio.tech.Models;
using api_desafio.tech.DTOs;
using api_desafio.tech.Helpers;
using System.Reflection;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;


namespace api_desafio.tech.EndPoints
{
    public static class AuthEndPoint
    {
        public static void AuthEndPoints(this WebApplication app)
        {
            var endpoint = app.MapGroup("auth");

            endpoint.MapPost("/login", async (LoginRequest request, AppDbContext context, TokenService tokenService, IPasswordHasher<User> passwordHasher, CancellationToken ct) =>
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
                if (user == null || !user.Active || passwordHasher.VerifyHashedPassword(user, user.Password, request.Password) == PasswordVerificationResult.Failed)
                {
                    return Results.BadRequest("Usuario inativo ou inexistente");
                }

                var token = tokenService.Generate(user);
                return Results.Ok(token);
            });

            endpoint.MapPost("/register", async (RegisterRequest request, IDistributedCache cache, IEmailService emailService, CancellationToken ct) =>
            {
                if (!ValidationHelpers.IsValidEmail(request.Email))
                {
                    return Results.BadRequest("Formato de e-mail inválido.");
                }

                if (!ValidationHelpers.IsValidPassword(request.Password))
                {
                    return Results.BadRequest("A senha deve ter no mínimo 8 caracteres, incluindo pelo menos uma letra maiúscula e um caractere especial.");
                }

                var userExists = await cache.GetStringAsync(request.Email, ct);
                if (userExists != null)
                {
                    return Results.Conflict("Já existe um usuário com este e-mail.");
                }

                var passwordHasher = new PasswordHasher<User>();
                var roles = request.Email == "admin@example.com" ? new string[] { "admin" } : new string[] { "user" };
                var newUser = new User(request.Name, request.Email, request.Password, roles);
                var hashedPassword = passwordHasher.HashPassword(newUser, request.Password);
                newUser.SetHashedPassword(hashedPassword);

                await VerificationCodeHelper.SendVerificationCodeAsync(newUser, newUser.Email, emailService, cache, ct);

                return Results.Ok("Usuário registrado. Verifique seu e-mail para ativar a conta.");
            });


            endpoint.MapPost("/verify", async (VerifyRequest request, AppDbContext context, IDistributedCache cache, CancellationToken ct) =>
            {
                var cachedCode = await cache.GetStringAsync($"{request.Email}_verificationCode", ct);
                var userName = await cache.GetStringAsync($"{request.Email}_name", ct);
                var userEmail = await cache.GetStringAsync($"{request.Email}_email", ct);
                var hashedPassword = await cache.GetStringAsync($"{request.Email}_hashedPassword", ct);

                if (cachedCode == null || cachedCode != request.Code)
                {
                    return Results.BadRequest("Código de verificação inválido ou expirado.");
                }

                if (userEmail == null || hashedPassword == null || userName == null)
                {
                    return Results.BadRequest("Usuário não encontrado ou dados expirados.");
                }

                var user = new User(userName, userEmail, hashedPassword, new string[] { "user" });

                context.Users.Add(user);
                await context.SaveChangesAsync(ct);

                await cache.RemoveAsync(request.Email, ct);
                await cache.RemoveAsync($"{request.Email}_verificationCode", ct);
                await cache.RemoveAsync($"{request.Email}_name", ct);
                await cache.RemoveAsync($"{request.Email}_email", ct);
                await cache.RemoveAsync($"{request.Email}_hashedPassword", ct);

                return Results.Ok("Verificação bem-sucedida. Usuário ativado.");
            });
        }
    }
}
