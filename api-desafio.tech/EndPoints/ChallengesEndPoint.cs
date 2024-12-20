using Microsoft.EntityFrameworkCore;
using api_desafio.tech.Data;
using System.Data;
using System.Security.Claims;
using api_desafio.tech.DTOs;
using api_desafio.tech.Requests;
using api_desafio.tech.Models;

namespace api_desafio.tech.EndPoints
{
    public static class ChallengesEndPoint
    {
        public static void AddChallengesEndpoints(this WebApplication app)
        {
            var endpoint = app.MapGroup("challenges").RequireAuthorization();

            endpoint.MapGet("/all", async (ClaimsPrincipal user, AppDbContext context, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var challenges = await context.Challenges.Where(challenge => challenge.UserId == userId).ToListAsync(ct);

                var challengeDtos = challenges.Select(challenge => new ChallengeDto(
                    challenge.Id,
                    challenge.Author,
                    challenge.Title,
                    challenge.Description,
                    challenge.StartDate,
                    challenge.EndDate,
                    challenge.ChallengeDates,
                    challenge.Completed,
                    challenge.UserId,
                    challenge.UserName
                )).ToList();

                return Results.Ok(challengeDtos);
            });

            endpoint.MapGet("/completed", async (ClaimsPrincipal user, AppDbContext context, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var challenges = await context.Challenges.Where(challenge => challenge.UserId == userId && challenge.Completed).ToListAsync(ct);

                var challengeDtos = challenges.Select(challenge => new ChallengeDto(
                    challenge.Id,
                    challenge.Author,
                    challenge.Title,
                    challenge.Description,
                    challenge.StartDate,
                    challenge.EndDate,
                    challenge.ChallengeDates,
                    challenge.Completed,
                    challenge.UserId,
                    challenge.UserName
                )).ToList();

                return Results.Ok(challengeDtos);
            });

            endpoint.MapGet("/notCompleted", async (ClaimsPrincipal user, AppDbContext context, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var challenges = await context.Challenges.Where(challenge => challenge.UserId == userId && !challenge.Completed).ToListAsync(ct);

                var challengeDtos = challenges.Select(challenge => new ChallengeDto(
                    challenge.Id,
                    challenge.Author,
                    challenge.Title,
                    challenge.Description,
                    challenge.StartDate,
                    challenge.EndDate,
                    challenge.ChallengeDates,
                    challenge.Completed,
                    challenge.UserId,
                    challenge.UserName
                )).ToList();

                return Results.Ok(challengeDtos);
            });

            endpoint.MapPost("create", async (ClaimsPrincipal user, CreateChallengeRequest request, AppDbContext context, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                var userNameClaim = user.FindFirst(ClaimTypes.Name);

                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }

                if (userNameClaim == null)
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var userName = userNameClaim.Value;
                var newChallenge = new Challenge(userName, request.Title, request.Description, request.StartDate);
                newChallenge.SetUserId(userId);
                newChallenge.SetUserName(userName);

                await context.Challenges.AddAsync(newChallenge, ct);
                await context.SaveChangesAsync(ct);

                var challengeDto = new ChallengeDto(
                    newChallenge.Id,
                    newChallenge.Author,
                    newChallenge.Title,
                    newChallenge.Description,
                    newChallenge.StartDate,
                    newChallenge.EndDate,
                    newChallenge.ChallengeDates,
                    newChallenge.Completed,
                    newChallenge.UserId,
                    newChallenge.UserName
                );

                return Results.Ok(challengeDto);
            });

            endpoint.MapPost("/cloneChallenge/{id:guid}", async (Guid id, ClaimsPrincipal user, AppDbContext context, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var challenge = await context.Challenges.FindAsync(id, ct);
                if (challenge == null)
                {
                    return Results.NotFound();
                }

                var newChallenge = new Challenge(
                    challenge.UserName ?? string.Empty,
                    challenge.Title ?? string.Empty,
                    challenge.Description ?? string.Empty,
                    DateTime.UtcNow
                );

                newChallenge.SetUserId(userId);
                newChallenge.SetUserName(challenge.UserName ?? string.Empty);

                await context.Challenges.AddAsync(newChallenge, ct);
                await context.SaveChangesAsync(ct);

                var challengeDto = new ChallengeDto(
                    newChallenge.Id,
                    newChallenge.Author,
                    newChallenge.Title,
                    newChallenge.Description,
                    newChallenge.StartDate,
                    newChallenge.EndDate,
                    newChallenge.ChallengeDates,
                    newChallenge.Completed,
                    newChallenge.UserId,
                    newChallenge.UserName
                );

                return Results.Ok(challengeDto);
            });

            endpoint.MapPut("edit/{id:guid}", async (Guid id, UpdateChallengeRequest request, ClaimsPrincipal user, AppDbContext context, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var challenge = await context.Challenges.FindAsync(id, ct);
                if (challenge == null)
                {
                    return Results.NotFound();
                }

                if (challenge.UserId != userId)
                {
                    return Results.Forbid();
                }

                challenge.UpdateChallenge(request.Title, request.Description, request.StartDate, request.Completed);

                await context.SaveChangesAsync(ct);

                var challengeDto = new ChallengeDto(
                    challenge.Id,
                    challenge.Author,
                    challenge.Title,
                    challenge.Description,
                    challenge.StartDate,
                    challenge.EndDate,
                    challenge.ChallengeDates,
                    challenge.Completed,
                    challenge.UserId,
                    challenge.UserName
                );

                return Results.Ok(challengeDto);
            });

            endpoint.MapDelete("delete/{id:guid}", async (Guid id, ClaimsPrincipal user, AppDbContext context, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var challenge = await context.Challenges.FindAsync(id, ct);
                if (challenge == null)
                {
                    return Results.NotFound();
                }

                if (challenge.UserId != userId)
                {
                    return Results.Forbid();
                }

                context.Challenges.Remove(challenge);
                await context.SaveChangesAsync(ct);
                return Results.Ok();
            });

        }
    }
}
