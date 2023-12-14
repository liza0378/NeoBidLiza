using Microsoft.EntityFrameworkCore;
using NeoBid.Server.Data.Entities;

namespace NeoBid.Server.Data.Helpers
{
    public static class BidHelper
    {
        public static async Task CreateBid(IDbContextFactory<ApplicationDbContext> dbFactory, Guid accountId, Guid auctionId, int amount)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var bid = new Bid
            {
                AccountId = accountId,
                Amount = amount,
                AuctionId = auctionId,
                CreatedOn = DateTime.Now,
            };


            context.Add(bid);

            await context.SaveChangesAsync();
        }
    }
}
