using NeoBid.Server.Data.Enums;

namespace NeoBid.Server.Data.Entities
{
    public class Auction
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte[] Image { get; set; }

        public DateTime CreatedOn { get; set; }

        public AuctionState State { get; set; }

        public Guid OwnerId { get; set; }

        public Account Owner { get; set; }

        public int FirstBid { get; set; }

        public virtual ICollection<Bid> Bids { get; set; }
    }
}
