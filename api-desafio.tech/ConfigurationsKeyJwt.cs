using DotNetEnv;

namespace api_desafio.tech
{
    public class ConfigurationsKeyJwt
    {
        public static string PrivateKey { get; private set; }

        static ConfigurationsKeyJwt()
        {
            Env.Load();

            PrivateKey = Environment.GetEnvironmentVariable("PRIVATE_KEY") ?? string.Empty;
        }
    }
}
