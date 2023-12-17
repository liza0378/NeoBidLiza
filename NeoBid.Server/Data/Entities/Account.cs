using NeoBid.Server.Data.Enums;

namespace NeoBid.Server.Data.Entities
{
    // Клас Account представляє обліковий запис користувача
    public class Account
    {
        public Guid Id { get; set; } // Унікальний ідентифікатор облікового запису

        public Role Role { get; set; } // Роль користувача User або Admin

        public byte[] Avatar { get; set; } // Аватар користувача у вигляді масиву байтів

        public string FullName { get; set; } // Повне ім'я користувача

        public string Email { get; set; } // Електронна пошта користувача

        public string Password { get; set; } // Хешований пароль користувача

        public virtual ICollection<Bid> Bids { get; set; } // Колекція ставок, зроблених користувачем
    }
}
