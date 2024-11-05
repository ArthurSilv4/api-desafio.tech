namespace api_desafio.tech.Models
{
    public class VerificationCode
    {
        public Guid Id { get; init; }
        public string Code { get; private set; }
        public DateTime Expiration { get; private set; }

        public Guid? UserId { get; private set; }
        public User? User { get; private set; }

        public VerificationCode(string code)
        {
            Id = Guid.NewGuid();
            Code = code;
            Expiration = DateTime.UtcNow.AddMinutes(15);
        }

        public void SetUserId(Guid userId)
        {
            UserId = userId;
        }
    }
}
