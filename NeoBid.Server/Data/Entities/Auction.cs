using NeoBid.Server.Data.Enums;

namespace NeoBid.Server.Data.Entities
{
    // Клас Auction представляє аукціон
    public class Auction
    {
        public Guid Id { get; set; } // Унікальний ідентифікатор аукціону

        public string Name { get; set; } // Назва аукціону

        public string Description { get; set; } // Опис аукціону

        public byte[] Image { get; set; } // Зображення аукціону у вигляді масиву байтів

        public DateTime CreatedOn { get; set; } // Дата та час створення аукціону

        public DateTime EndsOn { get; set; } // Дата та час закінчення аукціону

        public AuctionState State { get; set; } // Стан аукціону Draft, Open, Finished

        public Guid OwnerId { get; set; } // Ідентифікатор власника аукціону

        public Account Owner { get; set; } // Власник аукціону

        public int FirstBid { get; set; } // Початкова ставка аукціону

        public virtual ICollection<Bid> Bids { get; set; } // Колекція ставок у цьому аукціоні
    }
}
