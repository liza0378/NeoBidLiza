namespace NeoBid.Server.Data.DTOs
{
    // Клас AuctionFilterDto використовується для передачі критеріїв фільтрації аукціонів
    public class AuctionFilterDto
    {
        public DateTime From { get; set; } // Дата початку періоду для фільтрації аукціонів

        public DateTime To { get; set; } // Дата кінця періоду для фільтрації аукціонів

        public string Search { get; set; } // Текстовий рядок для пошуку аукціонів за назвою, описом тощо
    }
}
