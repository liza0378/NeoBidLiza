namespace NeoBid.Server.Data.Entities
{
    // Клас Order представляє замовлення, пов'язане зі ставкою на аукціоні
    public class Order
    {
        public Guid Id { get; set; } // Унікальний ідентифікатор замовлення

        public Guid BidId { get; set; } // Ідентифікатор ставки, на підставі якої було створено замовлення

        public virtual Bid Bid { get; set; } // Ставка, на підставі якої було створено замовлення

        public DateTime CreatedOn { get; set; } // Дата та час створення замовлення

        public string Address { get; set; } // Адреса доставки для замовлення
    }
}
