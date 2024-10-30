using Microsoft.EntityFrameworkCore;
using api_desafio.tech.Data;

namespace api_desafio.tech.Models.Challenges
{
    public static class ChallengesEndPoint
    {
        public static void AddChallengesEndpoints(this WebApplication app)
        {
            var endpoint = app.MapGroup("challenges");

            endpoint.MapGet("/all", async (AppDbContext context, CancellationToken ct) =>
            {
                var challenges = await context.Challenges.ToListAsync(ct);
                return Results.Ok(challenges);
            });

            endpoint.MapGet("/Completed", async (AppDbContext context, CancellationToken ct) =>
            {
                var challenges = await context.Challenges.Where(challenges => challenges.Completed).ToListAsync(ct);
                return Results.Ok(challenges);
            });

            endpoint.MapGet("/notCompleted", async (AppDbContext context, CancellationToken ct) =>
            {
                var challenges = await context.Challenges.Where(challenges => !challenges.Completed).ToListAsync(ct);
                return Results.Ok(challenges);
            });

            endpoint.MapPost("", async (AddChallengeRequest request, AppDbContext context, CancellationToken ct) =>
            {
                var newChallenge = new Challenge(request.Title, request.Description, request.StartDate);

                await context.Challenges.AddAsync(newChallenge, ct);
                await context.SaveChangesAsync(ct);


                return Results.Ok(newChallenge);
            });

            endpoint.MapPut("{id:guid}", async (Guid id, UpdateChallengeRequest request, AppDbContext context, CancellationToken ct) =>
            {
                var challenge = await context.Challenges.FindAsync(id, ct);
                if (challenge == null)
                {
                    return Results.NotFound();
                }

                challenge.UpdateChallenge(request.Title, request.Description, request.StartDate, request.Completed);

                await context.SaveChangesAsync(ct);
                return Results.Ok(challenge);
            });


            endpoint.MapDelete("{id:guid}", async (Guid id, AppDbContext context, CancellationToken ct) =>
            {
                var challenge = await context.Challenges.FindAsync(id, ct);
                if (challenge == null)
                {
                    return Results.NotFound();
                }

                context.Challenges.Remove(challenge);
                await context.SaveChangesAsync(ct);
                return Results.Ok();
            });
        }
    }
}
