using CryptoBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoBot.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Palabra> Palabras => Set<Palabra>();
    }
}
