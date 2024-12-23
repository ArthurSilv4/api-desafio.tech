using System.Text.Json.Serialization;

namespace api_desafio.tech.Models
{
    public class Mission
    {
        public Guid Id { get; init; }
        public Guid AuthorId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int Xp { get; private set; }
        public int Progress { get; private set; }
        public bool Completed { get; private set; }

        public Guid? UserId { get; private set; }

        [JsonIgnore]
        public User? User { get; private set; }

        private Mission() { }

        public Mission(Guid authorId, string title, string description, int xp, Guid userId)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Xp = xp;
            Progress = 0;
            Completed = false;
            UserId = userId;
            AuthorId = authorId;
        }

    }
}
