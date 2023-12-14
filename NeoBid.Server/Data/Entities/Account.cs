using NeoBid.Server.Data.Enums;

namespace NeoBid.Server.Data.Entities
{
    public class Account
    {
        public Guid Id { get; set; }

        public Role Role { get; set; }

        public byte[] Avatar { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public virtual ICollection<Bid> Bids { get; set; }
    }
}
