using api_desafio.tech.Models.Challenges;

namespace api_desafio.tech.Models.User
{
    public class User
    {
        public Guid Id { get; init; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string[] Roles { get; private set; }
        public bool Active { get; private set; }

        public ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();

        public User(string email, string password, string[] roles)
        {
            Id = new Guid();
            Email = email;
            Password = password;
            Roles = roles;
            Active = true;
        }

        public void SetHashedPassword(string hashedPassword)
        {
            Password = hashedPassword;
        }

        public void DisabeUser()
        {
            Active = false;
        }
    }
}
