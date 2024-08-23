using Microsoft.EntityFrameworkCore;
using GuestBookApp.Models;

namespace GuestBookApp.Data
{
    public class GuestBookContext : DbContext
    {
        public GuestBookContext(DbContextOptions<GuestBookContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Message> Messages { get; set; } = default!;
    }
}
