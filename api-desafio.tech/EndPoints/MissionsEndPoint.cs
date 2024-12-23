using api_desafio.tech.Data;
using api_desafio.tech.Models;
using api_desafio.tech.Requests;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api_desafio.tech.EndPoints
{
    public static class MissionsEndPoint
    {
        public static void MissionsEndPoints(this WebApplication app)
        {
            var endpoint = app.MapGroup("missions").RequireAuthorization();


            endpoint.MapGet("/list", async (AppDbContext context, CancellationToken ct) =>
            {
                var missions = await context.Missions.ToListAsync(ct);

                return Results.Ok(missions);
            });

            endpoint.MapPost("/create", async (ClaimsPrincipal user, MissionCreateRequest request, AppDbContext context, CancellationToken ct) =>
            {
             
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }

                var authorId = Guid.Parse(userIdClaim.Value);
                var users = await context.Users.ToListAsync(ct);
                var missions = new List<Mission>();

                foreach (var u in users)
                {
                    var newMission = new Mission(authorId, request.Title, request.Description, request.Xp, u.Id);
                    missions.Add(newMission);
                }

                await context.Missions.AddRangeAsync(missions, ct);
                await context.SaveChangesAsync(ct);               

                return Results.Ok(missions);
            
            });
        }

    }
}
