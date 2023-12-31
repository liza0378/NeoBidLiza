﻿// Визначаємо простір імен для перерахувань, пов'язаних з сервером NeoBid
namespace NeoBid.Server.Data.Enums
{
    // Перерахування Role визначає різні ролі користувачів у системі
    public enum Role
    {
        User,  // Роль "Користувач" - базова роль для стандартних користувачів
        Admin  // Роль "Адміністратор" - роль для користувачів з розширеними правами доступу та управління
    }
}
