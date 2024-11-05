namespace api_desafio.tech.Models
{
    public class Challenge
    {
        public Guid Id { get; init; }
        public string? Title { get; private set; }
        public string? Description { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public List<DateTime> ChallengeDates { get; private set; }
        public bool Completed { get; private set; }

        public Guid? UserId { get; private set; }
        public User? User { get; private set; }

        public Challenge(string title, string description, DateTime startDate)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            StartDate = startDate.Date;
            EndDate = startDate.Date.AddDays(4);
            ChallengeDates = Enumerable.Range(0, 5).Select(offset => startDate.Date.AddDays(offset)).ToList();
            Completed = false;
        }

        public void UpdateChallenge(string title, string description, DateTime startDate, bool completed)
        {
            Title = title;
            Description = description;
            StartDate = startDate.Date;
            EndDate = startDate.Date.AddDays(4);
            ChallengeDates = Enumerable.Range(0, 5).Select(offset => startDate.Date.AddDays(offset)).ToList();
            Completed = completed;
        }

        public void SetUserId(Guid userId)
        {
            UserId = userId;
        }
    }
}
