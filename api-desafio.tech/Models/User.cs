using System.Text.Json.Serialization;

namespace api_desafio.tech.Models
{
    public class User
    {
        public Guid Id { get; init; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string[] Roles { get; private set; }
        public bool Active { get; private set; }
        public bool ReceiveEmail { get; private set; }

        public int Xp { get; private set; }
        public int Level { get; private set; }
        public int Rank { get; private set; }
        public int ChallengesCompleted { get; private set; }
        public int MissionsCompleted { get; private set; }

        public List<string> SocialMedia { get; private set; }

        [JsonIgnore]
        public ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();

        [JsonIgnore]
        public ICollection<Mission> Missions { get; set; } = new List<Mission>();

        public User(string name, string email, string password, string[] roles)
        {
            Id = new Guid();
            Name = name;
            Description = null;
            Email = email;
            Password = password;
            Roles = roles;
            Xp = 0;
            Level = 0;
            Active = true;
            Rank = 0;
            ChallengesCompleted = 0;
            MissionsCompleted = 0;
            SocialMedia = new List<string>();
            ReceiveEmail = true;
        }

        public void UpdateEmail(string email)
        {
            Email = email;
        }

        public void SetHashedPassword(string hashedPassword)
        {
            Password = hashedPassword;
        }

        public void Disable()
        {
            Active = false;
        }

        public void ActiveUser()
        {
            Active = true;
        }
    }
}
