using Microsoft.EntityFrameworkCore;
using NeoBid.Server.Data.Entities;

namespace NeoBid.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Auction> Auctions { get; set; }

        public DbSet<Bid> Bids { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasIndex(x => x.Email).IsUnique();
        }
    }
}
