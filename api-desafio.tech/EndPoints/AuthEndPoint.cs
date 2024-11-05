using api_desafio.tech.Data;
using api_desafio.tech.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using api_desafio.tech.Requests;
using api_desafio.tech.Models;
using api_desafio.tech.DTOs;
using api_desafio.tech.Helpers;
using System.Reflection;


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
                return Results.Ok(new { Token = token });
            });

            endpoint.MapPost("/register", async (RegisterRequest request, AppDbContext context, IEmailService emailService, CancellationToken ct) =>
            {
                if (!ValidationHelpers.IsValidEmail(request.Email))
                {
                    return Results.BadRequest("Formato de e-mail inválido.");
                }

                if (!ValidationHelpers.IsValidPassword(request.Password))
                {
                    return Results.BadRequest("A senha deve ter no mínimo 8 caracteres, incluindo pelo menos uma letra maiúscula e um caractere especial.");
                }

                var userExists = await context.Users.AnyAsync(user => user.Email == request.Email, ct);

                if (userExists)
                {
                    return Results.Conflict("Já existe!");
                }

                var passwordHasher = new PasswordHasher<User>();
                var newUser = new User(request.Email, request.Password, request.Roles);
                var hashedPassword = passwordHasher.HashPassword(newUser, request.Password);
                newUser.SetHashedPassword(hashedPassword);
                newUser.DisabeUser();

                await context.Users.AddAsync(newUser, ct);
                await context.SaveChangesAsync(ct);

                var verificationCode = CodeGenerator.GenerateVerificationCode();
                var verificationCodeEntity = new VerificationCode(verificationCode);
                verificationCodeEntity.SetUserId(newUser.Id);

                await context.VerificationCodes.AddAsync(verificationCodeEntity, ct);
                await context.SaveChangesAsync(ct);

                await emailService.SendEmailAsync(newUser.Email, "Código de Verificação", $"Seu código de verificação é: {verificationCode}");

                var userReturn = new UserDto(newUser.Id, newUser.Email, newUser.Roles);
                return Results.Ok(userReturn);
            });

            endpoint.MapPost("/verify", async (VerifyRequest request, AppDbContext context, CancellationToken ct) =>
            {
                var verificationCode = await context.VerificationCodes.FirstOrDefaultAsync(vc => vc.UserId == request.UserId && vc.Code == request.Code, ct);

                if (verificationCode == null || verificationCode.Expiration < DateTime.UtcNow)
                {
                    return Results.BadRequest("Código de verificação inválido ou expirado.");
                }

                var user = await context.Users.FindAsync(request.UserId);
                if (user != null)
                {
                    user.ActiveUser();
                    await context.SaveChangesAsync(ct);
                }

                return Results.Ok("Verificação bem-sucedida.");
            });

            endpoint.MapPost("/resendVerification", async (string email, AppDbContext context, IEmailService emailService, CancellationToken ct) =>
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
                if (user == null)
                {
                    return Results.NotFound("Usuário não encontrado.");
                }

                if (user.Active)
                {
                    return Results.BadRequest("Usuário já está ativo.");
                }

                var verificationCode = CodeGenerator.GenerateVerificationCode();
                var verificationCodeEntity = new VerificationCode(verificationCode);
                verificationCodeEntity.SetUserId(user.Id);

                await context.VerificationCodes.AddAsync(verificationCodeEntity, ct);
                await context.SaveChangesAsync(ct);

                await emailService.SendEmailAsync(user.Email, "Código de Verificação", $"Seu novo código de verificação é: {verificationCode}");

                return Results.Ok("Novo código de verificação enviado com sucesso.");
            });
        }
    }
}
