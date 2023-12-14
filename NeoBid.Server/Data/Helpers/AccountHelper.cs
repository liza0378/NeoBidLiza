using Microsoft.EntityFrameworkCore;
using NeoBid.Server.Data.Entities;
using NeoBid.Server.Data.Enums;

namespace NeoBid.Server.Data.Helpers
{
    public static class AccountHelper
    {
        public static async Task<Account?> SignIn(IDbContextFactory<ApplicationDbContext> dbFactory, string login, string password)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var account = await context.Accounts.FirstOrDefaultAsync(x => x.Email == login);

            return account != null && BCrypt.Net.BCrypt.Verify(password, account.Password) ? account : null;
        }

        public static async Task<Account?> GetAccountByEmail(IDbContextFactory<ApplicationDbContext> dbFactory, string email)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            return await context.Accounts.SingleOrDefaultAsync(x=>x.Email == email);
        }

        public static async Task SignUp(IDbContextFactory<ApplicationDbContext> dbFactory, string login, string password, string fullname, byte[] avatar)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var account = new Account 
            { 
                Email = login,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                FullName = fullname,
                Avatar = avatar,
                Role = Role.User
            };

            context.Accounts.Add(account);

            await context.SaveChangesAsync();
        }
    }
}
