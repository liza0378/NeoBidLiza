namespace NeoBid.Server.Data.Entities
{
    public class Bid
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public Guid AuctionId { get; set; }

        public int Amount { get; set; }

        public DateTime CreatedOn {  get; set; }

        public virtual Auction Auction { get; set; }

        public virtual Account Account { get; set; }
    }
}
