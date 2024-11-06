using api_desafio.tech.Data;
using api_desafio.tech.Models;
using System.Threading;
using System.Threading.Tasks;
using api_desafio.tech.Services;

namespace api_desafio.tech.Helpers
{
    public static class VerificationCodeHelper
    {
        public static async Task SendVerificationCodeAsync(User user, AppDbContext context, IEmailService emailService, CancellationToken ct)
        {
            var verificationCode = CodeGenerator.GenerateVerificationCode();
            var verificationCodeEntity = new VerificationCode(verificationCode);
            verificationCodeEntity.SetUserId(user.Id);

            await context.VerificationCodes.AddAsync(verificationCodeEntity, ct);
            await context.SaveChangesAsync(ct);

            await emailService.SendEmailAsync(user.Email, "Código de Verificação", $"Seu novo código de verificação é: {verificationCode}");
        }
    }
}
