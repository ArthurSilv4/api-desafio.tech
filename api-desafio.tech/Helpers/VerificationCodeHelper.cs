using api_desafio.tech.Data;
using api_desafio.tech.Models;
using System.Threading;
using System.Threading.Tasks;
using api_desafio.tech.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace api_desafio.tech.Helpers
{
    public static class VerificationCodeHelper
    {
        public static async Task SendVerificationCodeAsync(User user, string email, IEmailService emailService, IDistributedCache cache, CancellationToken ct)
        {
            var verificationCode = CodeGenerator.GenerateVerificationCode();
            var cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)); // Expira em 10 minutos

            await cache.SetStringAsync($"{email}_verificationCode", verificationCode, cacheEntryOptions, ct);
            await cache.SetStringAsync($"{email}_name", user.Name, cacheEntryOptions, ct);
            await cache.SetStringAsync($"{email}_email", user.Email, cacheEntryOptions, ct);
            await cache.SetStringAsync($"{email}_hashedPassword", user.Password, cacheEntryOptions, ct);

            await emailService.SendEmailAsync(email, "Código de Verificação", $"Seu novo código de verificação é: {verificationCode}");
        }
    }
}
