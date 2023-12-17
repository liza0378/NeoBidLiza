using Microsoft.EntityFrameworkCore;
using NeoBid.Server.Data.Entities;

// Визначаємо простір імен для допоміжних функцій, пов'язаних із замовленнями
namespace NeoBid.Server.Data.Helpers
{
    // Статичний клас OrderHelper для роботи з замовленнями
    public static class OrderHelper
    {
        // Асинхронний метод для створення нового замовлення
        public static async Task CreateOrder(IDbContextFactory<ApplicationDbContext> dbFactory, Guid bidId, string address)
        {
            // Створюємо контекст бази даних
            using var context = await dbFactory.CreateDbContextAsync();

            // Створюємо новий об'єкт Order (замовлення)
            var order = new Order
            {
                BidId = bidId, // ID ставки, на підставі якої робиться замовлення
                Address = address, // Адреса доставки
                CreatedOn = DateTime.Now // Встановлюємо час створення замовлення
            };

            // Додаємо замовлення до контексту бази даних
            context.Orders.Add(order);

            // Зберігаємо зміни в базі даних
            await context.SaveChangesAsync();
        }

        // Метод для отримання списку замовлень користувача
        public static async Task<List<Order>> GetOrders(IDbContextFactory<ApplicationDbContext> dbFactory, Guid accountId)
        {
            // Створюємо контекст бази даних
            using var context = await dbFactory.CreateDbContextAsync();

            // Запит до бази даних для отримання замовлень, пов'язаних із ставками конкретного користувача
            return await context.Orders
                .Where(x => x.Bid.AccountId == accountId) // Фільтруємо замовлення за ID користувача
                .Include(x => x.Bid.Auction) // Включаємо інформацію про аукціон
                .ToListAsync(); // Виконуємо запит та повертаємо список замовлень
        }
    }
}
