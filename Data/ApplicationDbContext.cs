using Microsoft.EntityFrameworkCore;
using SplititActorsApi.Models;

namespace SplititActorsApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Actor> Actors { get; set; }
    }
}
