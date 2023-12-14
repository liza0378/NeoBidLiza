using Microsoft.EntityFrameworkCore;
using NeoBid.Server.Data.DTOs;
using NeoBid.Server.Data.Entities;
using NeoBid.Server.Data.Enums;

namespace NeoBid.Server.Data.Helpers
{
    public static class AuctionHelper
    {
        public static async Task<List<Auction>> GetAuctionsByHighestBidder(IDbContextFactory<ApplicationDbContext> dbFactory, Guid accountId)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var auctions = await context.Auctions
                .Where(a => a.Bids.Any()) // Ensure there are bids
                .Select(a => new
                {
                    Auction = a,
                    MaxBid = a.Bids.OrderByDescending(b => b.Amount).FirstOrDefault()
                })
                .Where(a => a.MaxBid.AccountId == accountId)
                .Select(a => a.Auction)
                .ToListAsync();

            return auctions;
        }

        public static async Task<List<Auction>> GetAuctionsForHome(IDbContextFactory<ApplicationDbContext> dbFactory, AuctionFilterDto filter)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var query = context.Auctions.Where(x=>x.CreatedOn >= filter.From && x.CreatedOn <= filter.To && x.State == AuctionState.Open);

            var list = await query.OrderByDescending(x=>x.CreatedOn).ToListAsync();

            return list;
        }

        public static async Task<List<Auction>> GetAuctionsWithMyBids(IDbContextFactory<ApplicationDbContext> dbFactory, Guid accountId)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var list = await context.Auctions.Where(x => x.Bids.Any(y=>y.AccountId == accountId)).ToListAsync();

            return list;
        }

        public static async Task<List<Auction>> GetAuctionsByAccount(IDbContextFactory<ApplicationDbContext> dbFactory, Guid accountId)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var list = await context.Auctions.Where(x=>x.OwnerId == accountId).ToListAsync();

            return list;
        }

        public static async Task<Auction?> GetAuctionById(IDbContextFactory<ApplicationDbContext> dbFactory, Guid auctionId)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            return await context.Auctions.FirstOrDefaultAsync(x => x.Id == auctionId);
        }

        public static async Task<Auction?> GetAuctionByIdForView(IDbContextFactory<ApplicationDbContext> dbFactory, Guid auctionId)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            return await context.Auctions
                .Include(x=>x.Owner)
                .Include(x=>x.Bids)
                .ThenInclude(x=>x.Account)
                .FirstOrDefaultAsync(x => x.Id == auctionId);
        }

        public static async Task EditAuction(IDbContextFactory<ApplicationDbContext> dbFactory, Auction auction)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            context.Update(auction);

            await context.SaveChangesAsync();
        }

        public static async Task DeleteAuction(IDbContextFactory<ApplicationDbContext> dbFactory, Auction auction)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            context.Remove(auction);

            await context.SaveChangesAsync();
        }

        public static async Task CreateAuction(IDbContextFactory<ApplicationDbContext> dbFactory, string name, string description, byte[] image, int firstBid, Guid ownerId)
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
                State = Enums.AuctionState.Draft
            };

            context.Auctions.Add(account);

            await context.SaveChangesAsync();
        }
    }
}
