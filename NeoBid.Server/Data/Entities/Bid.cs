namespace NeoBid.Server.Data.Entities
{
    // Клас Bid представляє ставку на аукціоні
    public class Bid
    {
        public Guid Id { get; set; } // Унікальний ідентифікатор ставки

        public Guid AccountId { get; set; } // Ідентифікатор користувача, який зробив ставку

        public Guid AuctionId { get; set; } // Ідентифікатор аукціону, для якого зроблена ставка

        public int Amount { get; set; } // Розмір ставки

        public DateTime CreatedOn { get; set; } // Дата та час створення ставки

        //Навігаційна властивість
        public virtual Auction Auction { get; set; } // Аукціон, для якого зроблена ставка

        public virtual Account Account { get; set; } // Користувач, який зробив ставку

        public virtual Order Order { get; set; } // Замовлення, пов'язане з цією ставкою
    }
}
