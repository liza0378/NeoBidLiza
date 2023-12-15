using Microsoft.EntityFrameworkCore;
using NeoBid.Server.Data.Entities;

namespace NeoBid.Server.Data.Helpers
{
    public static class OrderHelper
    {
        public static async Task CreateOrder(IDbContextFactory<ApplicationDbContext> dbFactory, Guid bidId, string address)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var order = new Order
            {
                BidId = bidId,
                Address = address,
                CreatedOn = DateTime.Now
            };
            
            context.Orders.Add(order);

            await context.SaveChangesAsync();
        }

        public static async Task<List<Order>> GetOrders(IDbContextFactory<ApplicationDbContext> dbFactory, Guid accounId)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            return await context.Orders.Where(x => x.Bid.AccountId == accounId).Include(x=>x.Bid.Auction).ToListAsync();
        }
    }
}
