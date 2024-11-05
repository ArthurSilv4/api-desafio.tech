using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using api_desafio.tech.Models;

namespace api_desafio.tech.Data
{
    public class AppDbContext :DbContext 
    {
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Env.Load();

            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("A connection string não foi encontrada na variável de ambiente 'DefaultConnection'.");
            }

            optionsBuilder.UseSqlServer(connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
