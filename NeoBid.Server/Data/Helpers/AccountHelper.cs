using Microsoft.EntityFrameworkCore;
using NeoBid.Server.Data.Entities;
using NeoBid.Server.Data.Enums;

// Визначаємо простір імен для допоміжних функцій, пов'язаних з обліковими записами
namespace NeoBid.Server.Data.Helpers
{
    // Статичний клас AccountHelper для роботи з обліковими записами
    public static class AccountHelper
    {
        // Асинхронний метод для входу в систему
        public static async Task<Account?> SignIn(IDbContextFactory<ApplicationDbContext> dbFactory, string login, string password)
        {
            // Створюємо контекст бази даних
            using var context = await dbFactory.CreateDbContextAsync();

            // Шукаємо обліковий запис за email
            var account = await context.Accounts.FirstOrDefaultAsync(x => x.Email == login);

            // Перевіряємо пароль та повертаємо обліковий запис або null
            return account != null && BCrypt.Net.BCrypt.Verify(password, account.Password) ? account : null;
        }

        // Асинхронний метод для отримання облікового запису за email
        public static async Task<Account?> GetAccountByEmail(IDbContextFactory<ApplicationDbContext> dbFactory, string email)
        {
            // Створюємо контекст бази даних
            using var context = await dbFactory.CreateDbContextAsync();

            // Повертаємо обліковий запис або null
            return await context.Accounts.SingleOrDefaultAsync(x => x.Email == email);
        }

        // Асинхронний метод для реєстрації нового облікового запису
        public static async Task SignUp(IDbContextFactory<ApplicationDbContext> dbFactory, string login, string password, string fullname, byte[] avatar)
        {
            // Створюємо контекст бази даних
            using var context = await dbFactory.CreateDbContextAsync();

            // Створюємо новий обліковий запис
            var account = new Account
            {
                Email = login,
                Password = BCrypt.Net.BCrypt.HashPassword(password), // Хешуємо пароль
                FullName = fullname,
                Avatar = avatar,
                Role = Role.User // Призначаємо роль користувача
            };

            // Додаємо обліковий запис до бази даних
            context.Accounts.Add(account);

            // Зберігаємо зміни в базі даних
            await context.SaveChangesAsync();
        }
    }
}
