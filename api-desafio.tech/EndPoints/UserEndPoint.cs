using api_desafio.tech.Data;
using api_desafio.tech.DTOs;
using api_desafio.tech.Helpers;
using api_desafio.tech.Models;
using api_desafio.tech.Requests;
using api_desafio.tech.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;

namespace api_desafio.tech.EndPoints
{
    public static class UserEndPoint
    {
        public static void AddUserEndPoints(this WebApplication app)
        {
            var endpoint = app.MapGroup("users").RequireAuthorization();

            endpoint.MapGet("/", async (ClaimsPrincipal user, AppDbContext context, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Results.NotFound();
                }

                var userEntity = await context.Users.FindAsync(new Guid(userIdClaim), ct);
                if (userEntity == null)
                {
                    return Results.NotFound();
                }

                var userDto = new UserDto(
                    userEntity.Id, 
                    userEntity.Name, 
                    userEntity.Description, 
                    userEntity.Email, 
                    userEntity.Roles, 
                    userEntity.Xp, 
                    userEntity.Level, 
                    userEntity.Rank,
                    userEntity.ChallengesCompleted,
                    userEntity.MissionsCompleted,
                    userEntity.SocialMedia,
                    userEntity.ReceiveEmail
                );

                return Results.Ok(userDto);
            });

            endpoint.MapPut("edit/email", async (ClaimsPrincipal user, UpdateUserRequest request, AppDbContext context, IDistributedCache cache, IEmailService emailService, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Results.NotFound();
                }

                var userEntity = await context.Users.FindAsync(new Guid(userIdClaim), ct);
                if (userEntity == null)
                {
                    return Results.NotFound();
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    return Results.BadRequest("Novo e-mail é obrigatório.");
                }

                if (!ValidationHelpers.IsValidEmail(request.Email))
                {
                    return Results.BadRequest("E-mail inválido.");
                }

                var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
                if (existingUser != null)
                {
                    return Results.BadRequest("E-mail já está em uso.");
                }

                await VerificationCodeHelper.SendVerificationCodeAsync(userEntity, request.Email, emailService, cache, ct);

                return Results.Ok("Código de confirmação enviado para o novo e-mail.");
            });

            endpoint.MapPut("edit/email/confirm", async (ClaimsPrincipal user, ConfirmEmailRequest request, AppDbContext context, IDistributedCache cache, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Results.NotFound();
                }

                var userEntity = await context.Users.FindAsync(new Guid(userIdClaim), ct);
                if (userEntity == null)
                {
                    return Results.NotFound();
                }

                if (string.IsNullOrEmpty(request.ConfirmationCode))
                {
                    return Results.BadRequest("Código de confirmação é obrigatório.");
                }

                var cachedCode = await cache.GetStringAsync($"{request.Email}_verificationCode", ct);
                if (cachedCode == null || cachedCode != request.ConfirmationCode)
                {
                    return Results.BadRequest("Código de confirmação inválido ou expirado.");
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    return Results.BadRequest("E-mail é obrigatório.");
                }

                userEntity.UpdateEmail(request.Email);
                
                await context.SaveChangesAsync(ct);

                await cache.RemoveAsync(request.Email, ct);
                await cache.RemoveAsync($"{request.Email}_verificationCode", ct);
                await cache.RemoveAsync($"{request.Email}_name", ct);
                await cache.RemoveAsync($"{request.Email}_email", ct);
                await cache.RemoveAsync($"{request.Email}_hashedPassword", ct);

                return Results.Ok("E-mail atualizado com sucesso.");
            });

            endpoint.MapDelete("disable", async (ClaimsPrincipal user, AppDbContext context, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Results.NotFound();
                }

                var userEntity = await context.Users.FindAsync(new Guid(userIdClaim), ct);
                if (userEntity == null)
                {
                    return Results.NotFound();
                }

                userEntity.Disable();

                await context.SaveChangesAsync(ct);
                return Results.Ok();
            });
        }
    }
}
