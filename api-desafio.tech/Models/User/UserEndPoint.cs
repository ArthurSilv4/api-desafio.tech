using api_desafio.tech.Data;

namespace api_desafio.tech.Models.User
{
    public static class UserEndPoint
    {
        public static void AddUserEndPoints(this WebApplication app)
        {
            var endpoint = app.MapGroup("users");

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
