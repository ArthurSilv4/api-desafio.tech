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

            endpoint.MapGet("/", async (AppDbContext context, CancellationToken ct) =>
            {
                var challenges = await context.Challenges.Where(challenge => !challenge.IsClone).ToListAsync(ct);

                var challengeDtos = challenges.Select(challenge => new ChallengeDto(
                    challenge.Id,
                    challenge.Author,
                    challenge.Title,
                    challenge.Description,
                    challenge.StartDate,
                    challenge.EndDate,
                    challenge.ChallengeDays,
                    challenge.Completed,
                    challenge.UserId,
                    challenge.UserName,
                    challenge.Status
                )).ToList();

                return Results.Ok(challengeDtos);
            });

            endpoint.MapGet("{id:guid}", async (Guid id, AppDbContext context, CancellationToken ct) =>
            {
                var challenge = await context.Challenges.FindAsync(id, ct);

                if (challenge == null)
                {
                    return Results.NotFound();
                }

                var challengeDto = new ChallengeDto(
                    challenge.Id,
                    challenge.Author,
                    challenge.Title,
                    challenge.Description,
                    challenge.StartDate,
                    challenge.EndDate,
                    challenge.ChallengeDays,
                    challenge.Completed,
                    challenge.UserId,
                    challenge.UserName,
                    challenge.Status
                );

                return Results.Ok(challengeDto);
            });

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
                    challenge.ChallengeDays,
                    challenge.Completed,
                    challenge.UserId,
                    challenge.UserName,
                    challenge.Status
                )).ToList();

                return Results.Ok(challengeDtos);
            });

            endpoint.MapGet("/all/{id:guid}", async (Guid id, ClaimsPrincipal user, AppDbContext context, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var challenge = await context.Challenges
                    .Where(challenge => challenge.UserId == userId && challenge.Id == id)
                    .FirstOrDefaultAsync(ct);

                if (challenge == null)
                {
                    return Results.NotFound();
                }

                var challengeDto = new ChallengeDto(
                    challenge.Id,
                    challenge.Author,
                    challenge.Title,
                    challenge.Description,
                    challenge.StartDate,
                    challenge.EndDate,
                    challenge.ChallengeDays,
                    challenge.Completed,
                    challenge.UserId,
                    challenge.UserName,
                    challenge.Status
                );

                return Results.Ok(challengeDto);
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
                    challenge.ChallengeDays,
                    challenge.Completed,
                    challenge.UserId,
                    challenge.UserName,
                    challenge.Status
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
                    challenge.ChallengeDays,
                    challenge.Completed,
                    challenge.UserId,
                    challenge.UserName,
                    challenge.Status
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
                var isclone = false;
                var newChallenge = new Challenge(userName, request.Title, request.Description, request.StartDate, isclone);
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
                    newChallenge.ChallengeDays,
                    newChallenge.Completed,
                    newChallenge.UserId,
                    newChallenge.UserName,
                    newChallenge.Status
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

                var isClone = true;
                var newChallenge = new Challenge(
                    challenge.UserName ?? string.Empty,
                    challenge.Title ?? string.Empty,
                    challenge.Description ?? string.Empty,
                    DateTime.UtcNow,
                    isClone
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
                    newChallenge.ChallengeDays,
                    newChallenge.Completed,
                    newChallenge.UserId,
                    newChallenge.UserName,
                    newChallenge.Status
                );

                return Results.Ok(challengeDto);
            });


            endpoint.MapPut("edit/days/{id:guid}", async (Guid id, UpdateChallengeDayRequest request, ClaimsPrincipal user, AppDbContext context, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }
                var userId = Guid.Parse(userIdClaim.Value);
                var challenge = await context.ChallengeDays.FindAsync(id, ct);
                if (challenge == null)
                {
                    return Results.NotFound();
                }

                challenge.UpdateDescription(request.Description);
                await context.SaveChangesAsync(ct);

                var challengeDayDto = new ChallengeDayDto(
                    challenge.Date,
                    challenge.Description ?? string.Empty
                );

                return Results.Ok(challengeDayDto);
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
                    challenge.ChallengeDays,
                    challenge.Completed,
                    challenge.UserId,
                    challenge.UserName,
                    challenge.Status
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
