using Microsoft.EntityFrameworkCore;
using NeoBid.Server.Data.Entities;

// Визначаємо простір імен для модуля даних NeoBid.Server
namespace NeoBid.Server.Data
{
    // Клас ApplicationDbContext, який розширює DbContext для роботи з базою даних
    public class ApplicationDbContext : DbContext
    {
        // Конструктор класу з параметрами для налаштувань контексту
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Властивості для доступу до таблиць бази даних
        public DbSet<Account> Accounts { get; set; } // Таблиця для облікових записів

        public DbSet<Auction> Auctions { get; set; } // Таблиця для аукціонів
        
        public DbSet<Bid> Bids { get; set; } // Таблиця для ставок
        
        public DbSet<Order> Orders { get; set; } // Таблиця для замовлень

        // Метод для конфігурації моделей при створенні моделі бази даних
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Налаштовуємо індекс для поля Email в таблиці Account, робимо його унікальним
            modelBuilder.Entity<Account>()
                .HasIndex(x => x.Email).IsUnique();
        }
    }
}
