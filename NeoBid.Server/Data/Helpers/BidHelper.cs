using Microsoft.EntityFrameworkCore;
using NeoBid.Server.Data.Entities;

// Визначаємо простір імен для допоміжних функцій, пов'язаних із ставками
namespace NeoBid.Server.Data.Helpers
{
    // Статичний клас BidHelper для роботи зі ставками
    public static class BidHelper
    {
        // Асинхронний метод для створення нової ставки
        public static async Task CreateBid(IDbContextFactory<ApplicationDbContext> dbFactory, Guid accountId, Guid auctionId, int amount)
        {
            // Створюємо контекст бази даних
            using var context = await dbFactory.CreateDbContextAsync();

            // Створюємо новий об'єкт Bid (ставку)
            var bid = new Bid
            {
                AccountId = accountId, // ID користувача, який робить ставку
                Amount = amount, // Розмір ставки
                AuctionId = auctionId, // ID аукціону, для якого робиться ставка
                CreatedOn = DateTime.Now, // Встановлюємо час створення ставки
            };

            // Додаємо ставку до контексту бази даних
            context.Add(bid);

            // Зберігаємо зміни в базі даних
            await context.SaveChangesAsync();
        }
    }
}
