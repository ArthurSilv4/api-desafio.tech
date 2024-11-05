namespace api_desafio.tech.Models
{
    public class User
    {
        public Guid Id { get; init; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string[] Roles { get; private set; }
        public bool Active { get; private set; }

        public ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();
        public ICollection<VerificationCode> VerificationCodes { get; set; } = new List<VerificationCode>();


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

        public void ActiveUser()
        {
            Active = true;
        }
    }
}
