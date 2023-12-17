using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeoBid.Server.Data;

// Оголошуємо простір імен для сервера NeoBid
namespace NeoBid.Server
{
    // Клас TimedHostedService, який є фоновим сервісом
    public class TimedHostedService : BackgroundService
    {
        private readonly ILogger<TimedHostedService> _logger; // Логер для реєстрації подій
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory; // Фабрика для створення контекстів бази даних

        // Конструктор, що ініціалізує логер та фабрику контекстів
        public TimedHostedService(ILogger<TimedHostedService> logger, IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _logger = logger;
            _dbFactory = dbFactory;
        }

        // Асинхронний метод, що виконується при запуску сервісу
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            await DoWork(); // Виконання роботи одразу при запуску

            // Створення таймера з інтервалом в 1 хвилину
            using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

            try
            {
                // Цикл, який повторює роботу до зупинки сервісу
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await DoWork();
                }
            }
            catch (OperationCanceledException)
            {
                // Повідомлення про зупинку сервісу
                _logger.LogInformation("Timed Hosted Service is stopping.");
            }
        }

        // Метод для виконання задачі сервісу
        private async Task DoWork()
        {
            try
            {
                // Створення контексту бази даних
                using var context = await _dbFactory.CreateDbContextAsync();

                // Отримання поточного часу
                var currentTime = DateTime.Now;

                // Оновлення стану аукціонів, які завершилися
                await context.Auctions
                    .Where(x => x.EndsOn <= currentTime)
                    .ExecuteUpdateAsync(x => x.SetProperty(y => y.State, Data.Enums.AuctionState.Finished));
            }
            catch (Exception ex)
            {
                // Реєстрація виняткових ситуацій
                _logger.LogInformation(ex.ToString());
            }
        }
    }
}
