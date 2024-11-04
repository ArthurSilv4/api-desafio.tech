using Microsoft.EntityFrameworkCore;
using api_desafio.tech.Data;
using System.Data;
using System.Security.Claims;

namespace api_desafio.tech.Models.Challenges
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
                    challenge.Title,
                    challenge.Description,
                    challenge.StartDate,
                    challenge.EndDate,
                    challenge.ChallengeDates,
                    challenge.Completed,
                    challenge.UserId
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
                    challenge.Title,
                    challenge.Description,
                    challenge.StartDate,
                    challenge.EndDate,
                    challenge.ChallengeDates,
                    challenge.Completed,
                    challenge.UserId
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
                    challenge.Title,
                    challenge.Description,
                    challenge.StartDate,
                    challenge.EndDate,
                    challenge.ChallengeDates,
                    challenge.Completed,
                    challenge.UserId
                )).ToList();

                return Results.Ok(challengeDtos);
            });

            endpoint.MapPost("create", async (ClaimsPrincipal user, AddChallengeRequest request, AppDbContext context, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var newChallenge = new Challenge(request.Title, request.Description, request.StartDate);
                newChallenge.SetUserId(userId);

                await context.Challenges.AddAsync(newChallenge, ct);
                await context.SaveChangesAsync(ct);

                return Results.Ok(newChallenge);
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
                    challenge.Title,
                    challenge.Description,
                    challenge.StartDate,
                    challenge.EndDate,
                    challenge.ChallengeDates,
                    challenge.Completed,
                    challenge.UserId
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
