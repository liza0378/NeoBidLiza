// Імпортуємо необхідні простори імен для роботи з авторизацією, контекстом бази даних, та іншими компонентами сервера
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoBid.Server;
using NeoBid.Server.Auth;
using NeoBid.Server.Components;
using NeoBid.Server.Data;

// Створюємо конструктор для веб-додатку
var builder = WebApplication.CreateBuilder(args);

// Ініціалізуємо сервіси
var services = builder.Services;

// Додаємо сервіси Razor Components та Interactive Server Components до контейнера
services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Налаштовуємо контекст бази даних, використовуючи SQLite
services.AddDbContextFactory<ApplicationDbContext>(x =>
    x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Реєструємо CustomAuthenticationStateProvider як сервіс для управління станом аутентифікації
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// Додаємо сервіс, що дозволяє каскадно передавати стан аутентифікації
builder.Services.AddCascadingAuthenticationState();

// Реєструємо сервіс для аутентифікації
builder.Services.AddScoped<AuthenticationService>();

// Додаємо сервіс, що виконується за певним графіком
services.AddHostedService<TimedHostedService>();

// Створюємо об'єкт веб-додатку
var app = builder.Build();

// Створюємо сферу для ініціалізації бази даних
using (var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider;
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

    try
    {
        // Створюємо та ініціалізуємо контекст бази даних
        using var context = provider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        // Логуємо помилки, що виникли під час міграції бази даних
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occured during migration");
        throw;
    }
}

// Налаштовуємо конвеєр обробки HTTP запитів
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // Використовуємо HSTS для підвищення безпеки (за замовчуванням 30 днів)
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Налаштовуємо маршрутизацію для Razor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Запускаємо веб-додаток
app.Run();
