using api_desafio.tech.Models;
using System.Text.Json.Serialization;

public class ChallengeDay
{
    public Guid Id { get; init; }
    public DateTime Date { get; private set; }
    public string? Description { get; private set; }
    public Guid ChallengeId { get; private set; }
    public Challenge? Challenge { get; private set; }

    public ChallengeDay(DateTime date, string? description, Guid challengeId)
    {
        Id = Guid.NewGuid();
        Date = date;
        Description = description;
        ChallengeId = challengeId;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
    }
}

public class Challenge
{
    public Guid Id { get; init; }
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    [JsonIgnore]
    public List<ChallengeDay> ChallengeDays { get; private set; }

    public bool Completed { get; private set; }
    public string Author { get; private set; }
    public string Status => Completed ? "Completo" : "Em Andamento";
    public bool IsClone { get; private set; }

    public Guid? UserId { get; private set; }
    public string? UserName { get; private set; }
    public User? User { get; private set; }

    public Challenge(string author, string title, string description, DateTime startDate, bool isClone)
    {
        Id = Guid.NewGuid();
        Author = author;
        Title = title;
        Description = description;
        StartDate = startDate.Date;
        EndDate = startDate.Date.AddDays(4);
        ChallengeDays = Enumerable.Range(0, 5).Select(offset => new ChallengeDay(startDate.Date.AddDays(offset), null, Id)).ToList();
        Completed = false;
        IsClone = isClone;
    }

    public void UpdateChallenge(string title, string description, DateTime startDate, bool completed)
    {
        Title = title;
        Description = description;
        StartDate = startDate.Date;
        EndDate = startDate.Date.AddDays(4);
        ChallengeDays = Enumerable.Range(0, 5).Select(offset => new ChallengeDay(startDate.Date.AddDays(offset), null, Id)).ToList();
        Completed = completed;
    }

    public void SetUserName(string userName)
    {
        UserName = userName;
    }

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }
}
