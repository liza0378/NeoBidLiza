namespace NeoBid.Server.Data.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid BidId { get; set; }

        public virtual Bid Bid { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Address { get; set; }
    }
}
