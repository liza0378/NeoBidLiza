using Microsoft.EntityFrameworkCore;
using NeoBid.Server.Data.DTOs;
using NeoBid.Server.Data.Entities;
using NeoBid.Server.Data.Enums;

// Визначаємо простір імен для допоміжних функцій, пов'язаних з аукціонами
namespace NeoBid.Server.Data.Helpers
{
    // Статичний клас AuctionHelper для роботи з аукціонами
    public static class AuctionHelper
    {
        // Метод для отримання аукціонів з найвищими ставками конкретного користувача
        public static async Task<List<(Auction Auction, Bid Bid)>> GetAuctionsByHighestBidder(IDbContextFactory<ApplicationDbContext> dbFactory, Guid accountId)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var auctions = await context.Auctions
                .Include(x => x.Bids) // Включаємо зв'язані ставки
                .ThenInclude(x => x.Order) // Включаємо зв'язані замовлення
                .Where(a => a.Bids.Any() && a.State == AuctionState.Finished) // Фільтруємо завершені аукціони
                .Select(a => new
                {
                    Auction = a,
                    MaxBid = a.Bids.OrderByDescending(b => b.Amount).FirstOrDefault() // Визначаємо найвищу ставку
                })
                .Where(a => a.MaxBid.AccountId == accountId) // Вибираємо ставки конкретного користувача
                .ToListAsync();

            return auctions.Select(x => (x.Auction, x.MaxBid)).ToList();
        }

        // Метод для отримання аукціонів для головної сторінки з фільтрами
        public static async Task<List<Auction>> GetAuctionsForHome(IDbContextFactory<ApplicationDbContext> dbFactory, AuctionFilterDto filter)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            // Формуємо запит з фільтрацією по датах та стану аукціону
            var query = context.Auctions.Where(x => x.EndsOn >= filter.From && x.EndsOn <= filter.To && x.State == AuctionState.Open);

            // Виконуємо запит та повертаємо список аукціонів
            var list = await query.OrderByDescending(x => x.CreatedOn).ToListAsync();

            return list;
        }

        // Метод для отримання аукціонів, в яких користувач робив ставки
        public static async Task<List<Auction>> GetAuctionsWithMyBids(IDbContextFactory<ApplicationDbContext> dbFactory, Guid accountId)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var list = await context.Auctions.Where(x => x.Bids.Any(y => y.AccountId == accountId)).ToListAsync();

            return list;
        }

        // Метод для отримання аукціонів конкретного користувача
        public static async Task<List<Auction>> GetAuctionsByAccount(IDbContextFactory<ApplicationDbContext> dbFactory, Guid accountId)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var list = await context.Auctions.Where(x => x.OwnerId == accountId).ToListAsync();

            return list;
        }

        // Метод для отримання конкретного аукціону по його ID
        public static async Task<Auction?> GetAuctionById(IDbContextFactory<ApplicationDbContext> dbFactory, Guid auctionId)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            return await context.Auctions.FirstOrDefaultAsync(x => x.Id == auctionId);
        }

        // Метод для отримання детальної інформації про аукціон для перегляду
        public static async Task<Auction?> GetAuctionByIdForView(IDbContextFactory<ApplicationDbContext> dbFactory, Guid auctionId)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            return await context.Auctions
                .Include(x => x.Owner) // Включаємо власника аукціону
                .Include(x => x.Bids) // Включаємо ставки
                .ThenInclude(x => x.Account) // Включаємо облікові записи, які робили ставки
                .FirstOrDefaultAsync(x => x.Id == auctionId);
        }

        // Метод для редагування аукціону
        public static async Task EditAuction(IDbContextFactory<ApplicationDbContext> dbFactory, Auction auction)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            context.Update(auction); // Оновлюємо аукціон

            await context.SaveChangesAsync(); // Зберігаємо зміни
        }

        // Метод для видалення аукціону
        public static async Task DeleteAuction(IDbContextFactory<ApplicationDbContext> dbFactory, Auction auction)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            context.Remove(auction); // Видаляємо аукціон

            await context.SaveChangesAsync(); // Зберігаємо зміни
        }

        // Метод для створення нового аукціону
        public static async Task CreateAuction(IDbContextFactory<ApplicationDbContext> dbFactory, string name, string description, byte[] image, int firstBid, Guid ownerId, DateTime endOn)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var account = new Auction
            {
                Name = name,
                Description = description,
                Image = image,
                CreatedOn = DateTime.Now,
                FirstBid = firstBid,
                OwnerId = ownerId,
                State = Enums.AuctionState.Draft,
                EndsOn = endOn
            };

            context.Auctions.Add(account); // Додаємо новий аукціон

            await context.SaveChangesAsync(); // Зберігаємо зміни
        }
    }
}
